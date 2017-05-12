namespace ControlPenales
{
    public enum enumProcesosEstudiosPersonalidad
    {

    };

    public enum eSINO
    {
        SI = 0,
        NO = 1
    };

    public enum eSINOProcesos
    {
        SI = 1,
        NO = 0
    };

    public enum eDiagnosticoDictamen
    {
        FAVORABLE = 1,
        DESFAVORABLE = 2
    };

    public enum eDatosGeograficosBase
    {
        MEXICO = 82
    }

    public enum eProcesoVentanasEstudioPersonalidad
    { 
        PROGRAMACION_ESTUDIOS = 1,
        CIERRE_ESTUDIOS = 2
    };

    public enum eMesesAnio
    {
        ENERO = 1,
        FEBRERO = 2,
        MARZO = 3,
        ABRIL = 4,
        MAYO = 5,
        JUNIO = 6,
        JULIO = 7,
        AGOSTO = 8,
        SEPTIEMBRE = 9,
        OCTUBRE = 10,
        NOVIEMBRE = 11,
        DICIEMBRE = 12
    };


    public enum eFueros
    {
        COMUN = 1,
        FEDERAL = 2
    };

    public enum eOrdenCaptura
    {
        PRIMERO = 0,
        SEGUNDO = 1,
        TERCERO = 2,
        CUARTO = 3,
        QUINTO = 4,
        SEXTO = 5,
        SEPTIMO = 6,
        OCTAVO = 7
    };


    public enum eTipopGrupoTrabajoSocial
    {
        PRIMARIO = 1,
        SECUNDARIO = 2
    };

    public enum eTipoActividad
    {
        APOYO_ESPIRITUAL_Y_RELIGIOSO = 9,
        FORTALECIMIENTO_NUCELO_FAMILIAR = 6,
        ALCOHLICOS_ANONIMOS = 1,
        DESINTOXICACION = 1,
        NUCLEO_FAM = 11
    }

    public enum eGrupos
    {
        PROGRAMAS_DESHABITUAMIENTO = 5,
        PROGRAMAS_MODIFIC_CONDUCTA = 6,
        COMPLEMENTARIO = 10,
        RECONSTRUCCION_PERSONAL = 4,
        BASICA = 1,
        MEDIA = 2,
        SUPERIOR = 3,
        RELIGIOSO = 9,
        TALLERES_ORIENTACION = 127 //NO EXISTE EN PROCESO
    };

    public enum eEstatusCausaPenal
    {
        ACTIVO = 1,
        SUSPENDIDO = 2,
        SOBRESEIDO = 3,
        CONCLUIDO = 4,
        POR_COMPURGAR = 0,
        INACTIVO = 5,
        EN_PROCESO = 6
    };

    public enum eBuenaRegularMala
    {
        BUENA = 1,
        REGULAR = 2,
        MALA = 3
    };

    public enum eCapacidadEstudio
    {
        ALTA = 1,
        MEDIA = 2,
        MEDIA_BAJA = 3,
        BAJA = 4
    };

    public enum eEMIFactorNivel
    {
        ALTO = 3,
        MEDIO = 2,
        MEDIO_BAJO = 4,
        BAJO = 1
    };

    public enum eMaxLengthCampos
    {
        CIEN = 100,
        DOSCIENTOS = 200,
        QUINIENTOS = 500
    };

    public enum eNivelIntelectual
    {
        SUPERIOR = 1,
        SUPERIOR_TERMINO_MEDIO = 2,
        MEDIO = 3,
        INFERIOR_TERMINO_MEDIO = 4,
        INFERIOR = 5,
        DEFICIENTE = 6
    };

    public enum eDisfuncionNeurologica
    {
        NO_PRESENTA = 1,
        SE_SOSPECHA = 2,
        DATOS_CLINICOS_EVIDENTES = 33
    };

    public enum eCapacidadCriminal
    {
        PRIMO_DELINCUENTE = 1,
        REINCIDENTE_ESPECIFICO = 2,
        REINCIDENTE_GENERICO = 3,
        HABITUAL = 4,
        PROFESIONAL = 5
    };

    public enum eCapacidad
    {
        ALTA = 1,
        MEDIA = 2,
        MEDIA_BAJA = 3,
        BAJA = 4
    };

    public enum ePeligrosidad
    {
        MAXIMA = 1,
        MEDIA_MAXIMA = 2,
        MEDIA = 3,
        MEDIA_MINIMA = 4,
        MINIMA = 5
    };

    public enum eAprobado
    {
        APROBADO = 1,
        NO_APROBADO = 2
    };

    public enum eTipoProceso
    {
        ACTIVIDAD_EDUCATIVA = 1,
        ESCOLARIDAD_ANTERIOR = 2
    };


    public enum eRolesCoordinadores
    {
        COORDINADOR_AREA_MEDICA = 29,
        COORDINADOR_PSIQUIATRICO = 40,
        COORDINADOR_PSICOLOGICO = 41,
        COORDINADOR_CRIMINODIAGNOSTICO = 42,
        COORDINADOR_TRABAJO_SOCAL = 43,
        COORDINADOR_EDUCATIVO = 44,
        COORDINADOR_PROGRAMAS = 45,
        COORDINADOR_COMANDANCIA = 46
    };

    public enum eRolesContemplados
    {
        PSIQUIATRA = 32,//ESTUDIO PSIQUIATRICO
        MEDICO = 30,//ESTUDIO MEDICO
        PSICOLOGO = 35,//ESTUDIO PSICOLOGICO
        CRIMINOLOGO = 22,//ESTUDIO CRIMINODIAGNOSTICO
        TRABAJO_SOCIAL = 34,//ESTUDIO SOCIO FAMILIAR
        EDUCATIVO = 37,//ESTUDIO EDUCATIVO, CULTURAL Y DEPORTIVO
        PROGRAMAS = 3, //ESTUDIO SOBRE CAPACITACION Y TRABAJO PENITENCIARIO
        COMANDANCIA = 12,//INFORME DEL AREA DE SEGURIDAD Y CUSTODIA,
        COORDINADOR_TECNICO = 27
    }

    public enum eTipoLista
    {
        CAPACITACION_LABORAL = 1,
        ACTIV_NO_GRATIFICADAS = 2,
        ACTIV_GRATIFICADAS = 3
    }

    public enum eEstatusSituacionPersonalidad
    {
        ACTIVO = 1,
        PENDIENTE = 2,
        TERMINADO = 3,
        CANCELADO = 4,
        ASIGNADO = 5
    }
}