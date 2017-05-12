using System.ComponentModel;

namespace ControlPenales
{
    partial class TrabajoSocialViewModel
    {
        //void ValidacionesLiberado()
        //{
        //    base.ClearRules();
        //    base.AddRule(() => SelectSexo, () => !string.IsNullOrEmpty(SelectSexo) && (SelectSexo == "F" || SelectSexo == "M"), "SEXO ES REQUERIDO!");
        //    base.AddRule(() => SelectOcupacion, () => SelectOcupacion!=null , "OCUPACION ES REQUERIDA!");
        //    base.AddRule(() => SelectReligion, () => SelectReligion!=null,"RELIGION ES REQUERIDA!");

        //}


        //void ValidacionesDatosEntrevista()
        //{
        //    base.ClearRules();
        //    base.AddRule(() => TextLugarEntrevista, () => !string.IsNullOrEmpty(TextLugarEntrevista) , "LUGAR ENTREVISTA ES REQUIERIDO!");
        //    base.AddRule(() => TextFechaEntrv, () => TextFechaEntrv!=null, "FECHA ENTREVISTAES ES REQUERIDO!");
        //    base.AddRule(() => TextNucEntrevista, () => string.IsNullOrEmpty(TextNucEntrevista) != null, "NUC ES REQUERIDO!");
        //    base.AddRule(() => TextCausaPenalEntrevista, () => string.IsNullOrEmpty(TextCausaPenalEntrevista) != null, "CAUSA PENAL ES REQUERIDO!");
        //}

        public void ValidacionesDatosTrabajoSocial()
        {
            base.ClearRules();
            base.AddRule(() => TextLugarEntrevista, () => !string.IsNullOrEmpty(TextLugarEntrevista), "LUGAR ENTREVISTA ES REQUIERIDO!");
            base.AddRule(() => TextFechaEntrv, () => TextFechaEntrv != null, "FECHA ENTREVISTA ES REQUERIDO!");
            base.AddRule(() => TextNucEntrevista, () => !string.IsNullOrEmpty(TextNucEntrevista), "NUC ES REQUERIDO!");
            base.AddRule(() => TextCausaPenalEntrevista, () => !string.IsNullOrEmpty(TextCausaPenalEntrevista), "CAUSA PENAL ES REQUERIDO!");
            base.AddRule(() => TextDelitoImputa, () => !string.IsNullOrEmpty(TextDelitoImputa), "DELITO QUE SE LE IMPUTA ES REQUERIDO!");
            base.AddRule(() => TextNombreApoyo, () => !string.IsNullOrEmpty(TextNombreApoyo), "NOMBRE QUE SE LE IMPUTA ES REQUERIDO!");
            base.AddRule(() => TextCalleApoyo, () => !string.IsNullOrEmpty(TextCalleApoyo), "CALLE ES REQUERIDA!");
            base.AddRule(() => TextNumeroInteriorApoyo, () => !string.IsNullOrEmpty(TextNumeroInteriorApoyo), "NUMERO INTERIOR ES REQUERIDO!");
            base.AddRule(() => TextNumeroExteriorApoyo, () => TextNumeroExteriorApoyo != null, "NUMERO EXTERIOR ES REQUERIDO!");
            base.AddRule(() => TextTelefonoApoyo, () => TextTelefonoApoyo != null ? TextTelefonoApoyo.Length >= 14 : false, "TELÉFONO APOYO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
            base.AddRule(() => TextEdadApoyo, () => TextEdadApoyo != null, "EDAD APOYO ES REQUERIDO!");
            base.AddRule(() => TextTiempoConocerceApoyo, () => !string.IsNullOrEmpty(TextTiempoConocerceApoyo), "TIEMPO DE CONOCER ES REQUERIDO!");
            base.AddRule(() => SelectParentesco, () => SelectParentesco.HasValue? SelectParentesco.Value>-1:false, "PARENTESCO ES REQUERIDO!");
            base.AddRule(() => SelectOcupacionApoyo, () => SelectOcupacionApoyo.HasValue?SelectOcupacionApoyo.Value > -1:false, "OCUPACIÓN ES REQUERIDO!");
            base.AddRule(() => TextDomicilioReferencia, () => !string.IsNullOrEmpty(TextDomicilioReferencia), "DOMICILIO REFERENCIA ES REQUERIDO!");
            
            
            
            OnPropertyChanged("TextLugarEntrevista");
            OnPropertyChanged("TextFechaEntrv");
            OnPropertyChanged("TextCalleApoyo");
            OnPropertyChanged("TextNucEntrevista");
            OnPropertyChanged("TextCausaPenalEntrevista");
            OnPropertyChanged("TextDelitoImputa");
            OnPropertyChanged("SelectOcupacionApoyo");
            OnPropertyChanged("TextNombreApoyo");
            OnPropertyChanged("TextNumeroInteriorApoyo");
            OnPropertyChanged("TextNumeroExteriorApoyo");
            OnPropertyChanged("TextTelefonoApoyo");
            OnPropertyChanged("TextEdadApoyo");
            OnPropertyChanged("TextTiempoConocerceApoyo");
            OnPropertyChanged("SelectParentesco");
            OnPropertyChanged("SelectOcupacionesApoyo");
            OnPropertyChanged("TextDomicilioReferencia");

        }
        void ValidacionSituacionActual()
        {
   
            base.AddRule(() => TextTiempoLibre, () => !string.IsNullOrEmpty(TextTiempoLibre), "A QUE DEDICA TIEMPO LIBRE ES REQUERIDO!");
            //base.AddRule(() => TextTiempoLibreOtro, () => !string.IsNullOrEmpty(TextTiempoLibreOtro), "OTRO TIEMPO LIBRE ES REQUERIDO!");
            //base.AddRule(() => TextEspecifiqueOtraEnfermedad, () => !string.IsNullOrEmpty(TextEspecifiqueOtraEnfermedad), "ESPECIFIQUE OTRA ENFERMEDAD ES REQUERIDO!");
         //   base.AddRule(() => TextTipoTratamientoRecibido, () => !string.IsNullOrEmpty(TextTipoTratamientoRecibido), "TIPO DE TRATAMIENTO RECIBIDO ES REQUERIDO!");
            base.AddRule(() => textDiagnostico, () => !string.IsNullOrEmpty(textDiagnostico), "DIAGNOSTICO ES REQUERIDO!");
            base.AddRule(() => ConoceVecinos, () => ConoceVecinosSi || ConoceVecinosNo, "SELECCIONE ALGUNA OPCIÓN");
            base.AddRule(() => ProblemaAlguno, () => ProblemasConVecinosSi || ProblemasConVecinosNo, "SELECCIONE ALGUNA OPCIÓN");
            //base.AddRule(() => TextDocumentosOtro, () => !string.IsNullOrEmpty(TextDocumentosOtro), "SELECCIONE ALGUNA OPCION");
            base.AddRule(() => PadecioEnfermedad, () => PadeceEnfermedadSi || PadeceEnfermedadNo, "SELECCIONE ALGUNA OPCIÓN");


            OnPropertyChanged("TextTiempoLibre");
            //OnPropertyChanged("TextTiempoLibreOtro");
           // OnPropertyChanged("TextEspecifiqueOtraEnfermedad");
           // OnPropertyChanged("TextTipoTratamientoRecibido");
            OnPropertyChanged("textDiagnostico");
            OnPropertyChanged("ConoceVecinos");
            OnPropertyChanged("ProblemaAlguno");
            OnPropertyChanged("PadecioEnfermedad");
            //OnPropertyChanged("TextDocumentosOtro");

        }

        void ValidacionEstructuraDinamica()
        {
            


            #region Estructura Dinamica

            //base.AddRule(() => TextFormaPorqueApoyo, () => !string.IsNullOrEmpty(TextFormaPorqueApoyo), "FORMA APOYO ES REQUERIDO!");
           // base.AddRule(() => DesdeCuandoVivePadres, () => !string.IsNullOrEmpty(DesdeCuandoVivePadres), "DESDE CUANDO VIVI CON PADRES ES REQUERIDO!");
            base.AddRule(() => MiembroFamiliaAbandono, () => MiembroFamiliaAbandonoHogarSi || MiembroFamiliaAbandonoHogarNo, "SELECCIONE ALGUNA OPCIÓN");
            base.AddRule(() => ProblemaFamiliar, () => ProblemaFamiliarNo || ProblemaFamiliarSi, "SELECCIONE ALGUNA OPCIÓN");
            base.AddRule(() => PadresVivenJuntos, () => ViveConPadres || ViveConPadresNo, "SELECCIONE ALGUNA OPCIÓN");

            base.AddRule(() => AntecedentesAnteriores, () => AntecedentesPernalesSi || AntecedentesPernalesNo, "SELECCIONE ALGUNA OPCIÓN");
            base.AddRule(() => ConsumidoDrogas, () => ConsumidoAlgunTipoDrogaSi || ConsumidoAlgunTipoDrogaNo, "SELECCIONE ALGUNA OPCIÓN");
            base.AddRule(() => RecibioApoyoprocesoJuducal, () => RecibioApoyoEconomicoEnProcesojudicialSi || RecibioApoyoEconomicoEnProcesojudicialNo, "SELECCIONE ALGUNA OPCIÓN");


            base.AddRule(() => DescrDinamicaFamiliar, () => !string.IsNullOrEmpty(DescrDinamicaFamiliar), "DESCRIPCIÓN APOYO ES REQUERIDO!");
            //base.AddRule(() => TextDeQuienRecibioApoyoInternamiento, () => !string.IsNullOrEmpty(TextDeQuienRecibioApoyoInternamiento), "RECIBIO APOYO  INTERNAMIENTO ES REQUERIDO!");
            base.AddRule(() => AntecedentesPenales, () => ExitenAntecedentespenalesFamiiarSi || ExitenAntecedentespenalesFamiiarNo, "RECIBIÓ APOYO  INTERNAMIENTO ES REQUERIDO!");
            base.AddRule(() => SustanciasToxicas, () => FamiliarConsumeSustanciaSi || FamiliarConsumeSustanciaNo, "SELECCIONE ALGUNA OPCIÓN!");
            base.AddRule(() => ApoyoDuranteInternamiento, () => RecibioApoyoInternamientoSi || RecibioApoyoInternamientoNo, "SELECCIONE ALGUNA OPCIÓN!");
            base.AddRule(() => ProblemaPareja, () => ProblemaParejaSi || ProblemaParejaNo, "SELECCIONE ALGUNA OPCIÓN!");

          //  base.AddRule(() => TextFrecuencia, () => !string.IsNullOrEmpty(TextFrecuencia), "FRECUENCIA ES REQUERIDO!");
            base.AddRule(() => TextunionesAnteriores, () => !string.IsNullOrEmpty(TextunionesAnteriores), "UNIONES ANTERIORES ES REQUERIDO!");
            base.AddRule(() => NoHijos, () => NoHijos != null, "UNIONES ANTERIORES ES REQUERIDO!");
            
          //  OnPropertyChanged("TextFormaPorqueApoyo");
           // OnPropertyChanged("DesdeCuandoVivePadres");
            OnPropertyChanged("PadresVivenJuntos");
            OnPropertyChanged("MiembroFamiliaAbandono");
            OnPropertyChanged("ProblemaFamiliar");
            //    OnPropertyChanged("PadresVivenjuntosHeader");
            OnPropertyChanged("DescrDinamicaFamiliar");
            //OnPropertyChanged("TextDeQuienRecibioApoyoInternamiento");
          //  OnPropertyChanged("TextFrecuencia");
            OnPropertyChanged("TextunionesAnteriores");
            OnPropertyChanged("NoHijos");

            OnPropertyChanged("RecibioApoyoprocesoJuducal");
            OnPropertyChanged("ConsumidoDrogas");

            OnPropertyChanged("AntecedentesAnteriores");
            OnPropertyChanged("AntecedentesPenales");
            OnPropertyChanged("SustanciasToxicas");
            OnPropertyChanged("ApoyoDuranteInternamiento");
            OnPropertyChanged("ProblemaPareja");
            #endregion
        }

        void ValidarNuceloFamiliar()
        {
  
            base.AddRule(() => TextNombreNuceloFamiliar, () => !string.IsNullOrEmpty(TextNombreNuceloFamiliar), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => TextEdadNuceloFamiliar, () => TextEdadNuceloFamiliar != null, "EDAD ES REQUERIDA!");
            base.AddRule(() => SelectEscolaridadNuceloFamiliar, () => SelectEscolaridadNuceloFamiliar > -1, "ESCOLARIDAD ES REQUERIDA!");
            base.AddRule(() => SelectEstadoCivilNuceloFamiliar, () => SelectEstadoCivilNuceloFamiliar > -1, "ESTADO CIVIL ES REQUERIDO!");
            base.AddRule(() => SelectParentescoNuceloFamiliar, () => SelectParentescoNuceloFamiliar > -1, "PARENTESCO ES REQUERIDO!");
            base.AddRule(() => SelectOcupacionNuceloFamiliar, () => SelectOcupacionNuceloFamiliar > -1, "OCUPACIÓN ES REQUERIDO!");
            base.AddRule(() => TextEdad, () => TextEdad != null, "EDAD ES REQUERIDO!");

            OnPropertyChanged("TextNombreNuceloFamiliar");
            OnPropertyChanged("TextEdadNuceloFamiliar");
            OnPropertyChanged("SelectEscolaridadNuceloFamiliar");
            OnPropertyChanged("SelectEstadoCivilNuceloFamiliar");
            OnPropertyChanged("SelectParentescoNuceloFamiliar");
            OnPropertyChanged("SelectOcupacionNuceloFamiliar");
            OnPropertyChanged("TextEdad");

        }
        void RemoverNuceloFamiliar()
        {
            base.RemoveRule("TextNombreNuceloFamiliar");
            OnPropertyChanged("TextNombreNuceloFamiliar");
            base.RemoveRule("TextEdadNuceloFamiliar");
            OnPropertyChanged("TextEdadNuceloFamiliar");
            base.RemoveRule("SelectEscolaridadNuceloFamiliar");
            OnPropertyChanged("SelectEscolaridadNuceloFamiliar");
            base.RemoveRule("SelectEstadoCivilNuceloFamiliar");
            OnPropertyChanged("SelectEstadoCivilNuceloFamiliar");
            base.RemoveRule("SelectParentescoNuceloFamiliar");
            OnPropertyChanged("SelectParentescoNuceloFamiliar");
            base.RemoveRule("SelectOcupacionNuceloFamiliar");
            OnPropertyChanged("SelectOcupacionNuceloFamiliar");
            base.RemoveRule("TextEdad");
            OnPropertyChanged("TextEdad");

        }

        void ValidacionEstudioSocioEconomico()
        {

            #region Estudio Socio Economico
  
            base.AddRule(() => TextTipoVivienda, () => !string.IsNullOrEmpty(TextTipoVivienda), "TIPO DE VIVIENDA ES REQUERIDO!");
            base.AddRule(() => TextNoPersonasResiden, () => TextNoPersonasResiden != null, "NUMERO PERSONAS RESIDEN ES REQUERIDO!");
            base.AddRule(() => TextPersonaViviaAntes, () => !string.IsNullOrEmpty(TextPersonaViviaAntes), "PERSONAS VIVÍA ANTES ES REQUERIDO!");
            //base.AddRule(() => TextOtrasPersonasViviaAntes, () => !string.IsNullOrEmpty(TextOtrasPersonasViviaAntes), "OTRAS PERSONAS VIVIA ANTES ES REQUERIDO!");
            base.AddRule(() => TextTipoMaterialVivienda, () => !string.IsNullOrEmpty(TextTipoMaterialVivienda), "TIPO MATERIAL VIVIENDA ES REQUERIDO!");
            base.AddRule(() => TextCondicionesZona, () => !string.IsNullOrEmpty(TextCondicionesZona), "CONDICIONES DE VIVIENDA ES REQUERIDO!");
            base.AddRule(() => TextSituacionEconomica, () => !string.IsNullOrEmpty(TextSituacionEconomica), "SITUACION ECONÓMICA ES REQUERIDO!");
            base.AddRule(() => TextEgresoLuz, () => TextEgresoLuz != null, "EGRESO LUZ ES REQUERIDO!");
            base.AddRule(() => TextEgresoEducacion, () => TextEgresoEducacion != null, "EGRESO EDUCACIÓN ES REQUERIDO!");
            base.AddRule(() => TextEgresoCombustible, () => TextEgresoCombustible != null, "EGRESO COMBUSTIBLE ES REQUERIDO!");
            base.AddRule(() => TextEgresoAgua, () => TextEgresoAgua != null, "EGRESO AGUA ES REQUERIDO!");
            base.AddRule(() => TextEgresoGas, () => TextEgresoGas != null, "EGRESO GAS ES REQUERIDO!");
            base.AddRule(() => TextEgresoTelefono, () => TextEgresoTelefono != null, "EGRESO TELÉFONO ES REQUERIDO!");
            base.AddRule(() => TextEgresoVestimenta, () => TextEgresoVestimenta != null, "EGRESO VESTIMENTA ES REQUERIDO!");
            base.AddRule(() => TextEgresoGatosMedicos, () => TextEgresoGatosMedicos != null, "EGRESO GATOS MEDICOS ES REQUERIDO!");
            base.AddRule(() => TextEgresoDespensa, () => TextEgresoDespensa != null, "EGRESO DESPENSA ES REQUERIDO!");
            base.AddRule(() => TextEgresoLeche, () => TextEgresoLeche != null, "EGRESO LECHE ES REQUERIDO!");
            base.AddRule(() => TextEgresoFrijol, () => TextEgresoFrijol != null, "EGRESO FRIJOL ES REQUERIDO!");
            base.AddRule(() => TextEgresoCarneRoja, () => TextEgresoCarneRoja != null, "EGRESO CARNE ROJA ES REQUERIDO!");
            base.AddRule(() => TextEgresoPastas, () => TextEgresoPastas != null, "EGRESO PASTAS ES REQUERIDO!");
            base.AddRule(() => TextEgresoLeguiminosas, () => TextEgresoLeguiminosas != null, "EGRESO LEGUIMINOSAS ES REQUERIDO!");
            base.AddRule(() => TextEgresoTortillas, () => TextEgresoTortillas != null, "EGRESO FRIJOL ES REQUERIDO!");
            base.AddRule(() => TextEgresoPolllo, () => TextEgresoPolllo != null, "EGRESO POLLO ES REQUERIDO!");
            base.AddRule(() => TextEgresoCereales, () => TextEgresoCereales != null, "EGRESO CEREALES  ES REQUERIDO!");
            base.AddRule(() => TextEgresoverduras, () => TextEgresoverduras != null, "EGRESO VERDURAS ES REQUERIDO!");
            base.AddRule(() => TextEgresoGolosinas, () => TextEgresoGolosinas != null, "EGRESO GOLOSINAS ES REQUERIDO!");
            //base.AddRule(() => TextEgresoOtros, () => TextEgresoOtros != null, "EGRESO OTROS ES REQUERIDO!");
            //base.AddRule(() => TextEgresoDescrOtros, () => TextEgresoDescrOtros != null, "EGRESO OTROS DESCRIPCION ES REQUERIDO!");
            base.AddRule(() => TextComidasAlDia, () => TextComidasAlDia != null, "COMIDAS AL DIA ES REQUERIDO!");
            base.AddRule(() => TextEgresoRenta, () => TextEgresoRenta != null, "EGRESO RENTA  ES REQUERIDO!");

            #region Estructura de la Vivivenda

            base.AddRule(() => TextComedorNum, () => TextComedorNum != null, "NUMERO COMEDORES ES REQUERIDO!");
            base.AddRule(() => TextComedorObserv, () => !string.IsNullOrEmpty( TextComedorObserv), "OBSERVACIÓN COMEDORES ES REQUERIDO!");

            base.AddRule(() => TextRecamaraNum, () => TextRecamaraNum != null, "NUMERO RECAMARAS ES REQUERIDO!");
            base.AddRule(() => TextRecamaraObserv, () => !string.IsNullOrEmpty(TextRecamaraObserv), "OBSERVACIÓN RECAMATRASES REQUERIDO!");


            base.AddRule(() => TextSalaNum, () => TextSalaNum != null, "NUMERO SALAS ES REQUERIDO!");
            base.AddRule(() => TextSalaObserv, () => !string.IsNullOrEmpty(TextSalaObserv), "OBSERVACIÓN SALAS REQUERIDO!");

            base.AddRule(() => TextCocinaNum, () => TextCocinaNum != null, "NUMERO COCINAS ES REQUERIDO!");
            base.AddRule(() => TextCocinaObserv, () => !string.IsNullOrEmpty(TextCocinaObserv), "OBSERVACIÓN COCINAS REQUERIDO!");

            base.AddRule(() => TextBañoNum, () => TextBañoNum != null, "NUMERO BAÑOS ES REQUERIDO!");
            base.AddRule(() => TextBañoObserv, () => !string.IsNullOrEmpty(TextBañoObserv), "OBSERVACIÓN BAÑOS REQUERIDO!");

            base.AddRule(() => TextVentanasNum, () => TextVentanasNum != null, "NUMERO VENTANAS ES REQUERIDO!");
            base.AddRule(() => TextVentanasObserv, () => !string.IsNullOrEmpty(TextVentanasObserv), "OBSERVACIÓN VENTANAS REQUERIDO!");
            
            base.AddRule(() => TextPatioNum, () => TextPatioNum != null, "NUMERO PATIOS ES REQUERIDO!");
            base.AddRule(() => TextPatioObserv, () => !string.IsNullOrEmpty(TextPatioObserv), "OBSERVACIÓN PATIOSREQUERIDO!");

            OnPropertyChanged("TextComedorNum");
            OnPropertyChanged("TextComedorObserv");
            OnPropertyChanged("TextRecamaraNum");
            OnPropertyChanged("TextRecamaraObserv");
            OnPropertyChanged("TextSalaNum");
            OnPropertyChanged("TextSalaObserv");
            OnPropertyChanged("TextCocinaNum");
            OnPropertyChanged("TextCocinaObserv");
            OnPropertyChanged("TextBañoNum");
            OnPropertyChanged("TextBañoObserv");
            OnPropertyChanged("TextVentanasNum");
            OnPropertyChanged("TextVentanasObserv");
            OnPropertyChanged("TextPatioNum");
            OnPropertyChanged("TextPatioObserv");

            

            #endregion


            //  base.AddRule(() => TextEgresoTotal, () => TextEgresoTotal != null, "EGRESO TOTAL ES REQUERIDO!");
            OnPropertyChanged("TextNoPersonasResiden");
            OnPropertyChanged("TextPersonaViviaAntes");
          //  OnPropertyChanged("TextOtrasPersonasViviaAntes");
            OnPropertyChanged("TextTipoMaterialVivienda");
            OnPropertyChanged("TextTipoMaterialVivienda");
            OnPropertyChanged("TextCondicionesZona");
            OnPropertyChanged("TextSituacionEconomica");
            OnPropertyChanged("TextEgresoLuz");
            OnPropertyChanged("TextEgresoFrijol");
            OnPropertyChanged("TextEgresoEducacion");
            OnPropertyChanged("TextEgresoCombustible");
            OnPropertyChanged("TextEgresoAgua");
            OnPropertyChanged("TextEgresoGas");
            OnPropertyChanged("TextEgresoTelefono");
       //     OnPropertyChanged("TextEgresoDescrOtros");
            OnPropertyChanged("TextEgresoCarneRoja");
            OnPropertyChanged("TextEgresoVestimenta");
            OnPropertyChanged("TextEgresoGatosMedicos");
            OnPropertyChanged("TextEgresoDespensa");
            OnPropertyChanged("TextEgresoPastas");
            OnPropertyChanged("TextEgresoLeche");
            OnPropertyChanged("TextEgresoLeguiminosas");
            OnPropertyChanged("TextEgresoTortillas");
            OnPropertyChanged("TextEgresoPolllo");
            OnPropertyChanged("TextEgresoCereales");
            OnPropertyChanged("TextTipoVivienda");
            OnPropertyChanged("TextEgresoverduras");
            OnPropertyChanged("TextEgresoGolosinas");
          //  OnPropertyChanged("TextEgresoOtros");
            OnPropertyChanged("TextComidasAlDia");
            OnPropertyChanged("TextEgresoRenta");
          //  OnPropertyChanged("TextEgresoTotal");


            #endregion
        }

    


        #region ApoyoEconomicoFamiliarPopup
        void ValidacionApoyoEconomicoFamiliar() 
        {


    
            base.AddRule(() => TextNombreFamiliar, () => !string.IsNullOrEmpty(TextNombreFamiliar), "NOMBRE ES REQUERIDO!");
            base.AddRule(() => SelectOcupacionApoyoEconomic, () => SelectOcupacionApoyoEconomic>-1, "OCUPACION ES REQUERIDO!");
            base.AddRule(() => TextAportaciones, () => !string.IsNullOrEmpty(TextAportaciones), "APORTACION ES REQUERIDA!");
            OnPropertyChanged("TextNombreFamiliar");
            OnPropertyChanged("SelectOcupacionApoyoEconomic");
            OnPropertyChanged("TextAportaciones");
        }
        void RemoverValidacionApoyoEconomicoFamiliar()
        {
            base.RemoveRule("TextNombreFamiliar");
            base.RemoveRule("SelectOcupacionApoyoEconomic");
            base.RemoveRule("TextAportaciones");
            OnPropertyChanged("TextNombreFamiliar");
            OnPropertyChanged("SelectOcupacionApoyoEconomic");
            OnPropertyChanged("TextAportaciones");
        }
        #endregion

        void ValidacionDrogafrecuencia()
        {
            
            base.AddRule(() => popUpFechaUltDosis, () => popUpFechaUltDosis != null, "ÚLTIMA DOSIS ES REQUERIDO!");
            base.AddRule(() => popUpFrecuenciaUso, () => popUpFrecuenciaUso > -1, "FRECUENCIA USO ES REQUERIDO!");
            base.AddRule(() => popUpDrogaId, () => popUpDrogaId > -1, "SELECCIÓN DE DROGA ES REQUERIDO!");


            OnPropertyChanged("popUpFechaUltDosis");
            OnPropertyChanged("popUpFrecuenciaUso");
            OnPropertyChanged("popUpDrogaId");

        }

        void RemoverValidacionDrogafrecuencia()
        {
            base.RemoveRule("popUpFechaUltDosis");
            base.RemoveRule("popUpFrecuenciaUso");
            base.RemoveRule("popUpDrogaId");
            OnPropertyChanged("popUpFechaUltDosis");
            OnPropertyChanged("popUpFrecuenciaUso");
            OnPropertyChanged("popUpDrogaId");
        }



    }
}
