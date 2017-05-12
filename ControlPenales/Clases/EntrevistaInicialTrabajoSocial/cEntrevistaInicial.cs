using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cEntrevistaInicial
    {
        #region Entrevista Inical Trabajo
        public string Nuc { get; set; }
        public string FechaEntrv { get; set; }
        public string LugarEntrv { get; set; }
        public string CausaPenalEntrv { get; set; }
        #endregion

        #region Datos Generales
        public string NombreInternoInicial { get; set; }
        public string NombreInterno { get; set; }
        public string NombreCentro { get; set; }
        public string FechaNacimiento { get; set; }
        public string EdadInterno { get; set; }
        public string Ocupacion { get; set; }
        public string SalarioPercibia { get; set; }
        public string Escolaridad { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Apodo { get; set; }
        public string Alias{ get; set; }
        public string EstadoCivil { get; set; }
        public string Municipio { get; set; }
        public string Pais{ get; set; }
        public string Estado { get; set; }



        public string GrupoEtnico { get; set; }
        public string TiempoAntiguedad { get; set; }
        public string oficio { get; set; }

        public string UltimoGradoEstudio { get; set; }

        public string Religion { get; set; }

        
        public string Idioma { get; set; }

        public string LugarNacimiento { get; set; }
        public string TiempoRadicaEstado { get; set; }

        public string Sexo { get; set; }

        #region Delitos
        public string Delito { get; set; }
        
        #endregion

        #region Domiclioreferencia(si el imputado tiene algun otro domicilio)
        public string DomicilioReferencia { get; set; }
        #endregion
        #region DatosPersonaApoyo
        public string NombreAoyo { get; set; }
        public string DomicilioAoyo { get; set; }
        public string OcupacionAoyo { get; set; }
        public string TelefonoAoyo { get; set; }

        public string CalleAoyo { get; set; }
        public string EdadApoyo { get; set; }
        

        public string ParentescoAoyo { get; set; }

        public string TiempoConocerAoyo { get; set; }
        #endregion
        #endregion

        #region EstudioSocio Economico
        public string PersonasResidenVivienda { get; set; }
        public string TipoVivienda { get; set; }

        public string PersonaViviaAntesSerDtenido { get; set; }
        public string PersonaViviaAntesSerDtenidoOtra { get; set; }

        public string TipoMterialVivienda { get; set; }

        public string CondicionZona { get; set; }

        public string SituacionEconomica { get; set; }

   

        #region Egresoso de Gastos

        public string LuzE { get; set; }
        public string EducacionE { get; set; }
        public string CombustibleE { get; set; }
        public string AguaE { get; set; }
        public string TelefonoE { get; set; }
        public string GastosMedicosE { get; set; }
        public string RentaE{ get; set; }
        public string GasE{ get; set; }
        public string vestimentaE { get; set; }
        public string DespensaE { get; set; }
        public string LugarE { get; set; }
        public string LecheE { get; set; }
        public string LeguiminosasE { get; set; }
        public string FrijolE { get; set; }
        public string TortillasE { get; set; }

        public string PolloE { get; set; }

       public string PastasE { get; set; }
       public string cerealesE { get; set; }

       public string VerdurasE { get; set; }

       public string CarneE { get; set; }

       public string GolosinasE { get; set; }

       public string TotalEgresos { get; set; }

       public string OtroEgreso { get; set; }
       public string OtroEgresoDescr { get; set; }


       public string ComidaAlDiaE { get; set; }


        #endregion

        #endregion

        #region EStructura Dinamica Familiar
       public string PadresVIvenJuntos { get; set; }
       public string MiembroFamiliarAbandonoHogar{ get; set; }
       public string ExistioProblemaFamiliar { get; set; }

       public string DescrDinamicaFamiliar { get; set; }
       public string RecibidoApoyoFamiliarDuranteProcesoJudicial { get; set; }

       

       public string DesdeCuandoViveConPadres { get; set; }

       public string FomraPoruqe { get; set; }

       public string AntecedentesMiembroFmiliar { get; set; }

       public string FmiliaresConsumanSustanciasToxicas { get; set; }
       public string ConsumeAlgunTipoDroga{ get; set; }
       public string AntecedentesPenalesAlActual { get; set; }
       public string RecibioApoyoDuranteInternamiento { get; set; }
       public string DeQuienRecibioApoyo { get; set; }
       public string Frecuencia { get; set; }

       public string UnionesAnteriores{ get; set; }
       public string NoHiijos{ get; set; }

       public string TieneProblemasPareja{ get; set; }

      



       #endregion

       #region SituacionActual
       public string ConoceVecinos { get; set; }
       public string ProblemasVecionos { get; set; }
       public string DedicaTiempoLibre { get; set; }

       public string DedicaTiempoLibreOtro { get; set; }

        #region Documentos Personales
       public string ActaNacimiento { get; set; }
       public string ComprobanteEstudios { get; set; }

       public string ComprobanteEstudio { get; set; }

       public string Licencia { get; set; }

       public string ActaMatrimonio { get; set; }

       public string PasaporteMexicano { get; set; }

       public string Ife { get; set; }

       public string cartilla { get; set; }

       public string VisaLaser { get; set; }

       public string Curp { get; set; }

       public string OtroDocumento{ get; set; }
        #endregion


        #endregion

        #region Salud
       public string PadecePadecioEnfermedad { get; set; }

       public string PadecePadecioEnfermedadOtra { get; set; }
       public string EspecifiqueEnfermedad { get; set; }

       public string TipoTRatamientoRecibido { get; set; }
       public string EntidadFederativa { get; set; }
       public string Diagnostico { get; set; }
       public string Observacion { get; set; }
        #endregion

       public string Fecha{ get; set; }

        #region Entrevista Domiciliaria
       public string MedidaCautelar { set; get; }
       public string HoraEntrv { set; get; }
       #region DatospersonaEntrevistada
       
       public string MotivooEntrv { set; get; }
       public string EstadoEntrv { set; get; }
       public string MunicioioEntrv { set; get; }
       public string EntidadFederativaEntrv { set; get; }


       #region Datos Persona entrevistada
       public string NombreEntrevistado { set; get; }
       public string EaddEntrevistado { set; get; }
       public string ParentescoEntrevistado { set; get; }
       public string TiempoConocerceEntrevistado { set; get; }
       public string DomicilioConocerceEntrevistado { set; get; }
       public string TelefonoConocerceEntrevistado { set; get; }
       public string EntidadfederativaEntrevistado { set; get; }
       public string MunicipioEntrevistado { set; get; }
       public string RelacionSentenciadoEntrevistado { set; get; }
       #endregion
       #endregion
       #region Croquis

       public string NombreCroquis { set; get; }
       public string DireccionCroquis { set; get; }
       public string TelefonoCroquis { set; get; }

       public byte[] FotografiasAnexo2 { set; get; }
        #endregion

       #region Fotografias
       public byte[] Fotografias1 { set; get; }
       public byte[] Fotografias2 { set; get; }
       public byte[] Fotografias3 { set; get; }
       public byte[] Fotografias4 { set; get; }
       #endregion

       #endregion
       #region Reporte Psicologico

       public string RadicadoEnBc { set; get; }
       public string TecnicasUtilizadas { set; get; }
       public string DescripcionEntrv { set; get; }
       public string ExamenMental { set; get; }
       public string Personalidad { set; get; }
       public string NuceloFamPrimario { set; get; }
       public string NucleoFamSecundario { set; get; }
       public string Sugerencias { set; get; }

       

        #endregion



    }
}
