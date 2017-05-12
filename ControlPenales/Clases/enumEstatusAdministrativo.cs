namespace ControlPenales
{
    public enum enumEstatusAdministrativo
    {
        EN_AREA_TEMPORAL = 1,
        EN_CLASIFICACION = 2,
        ASIGNADO_A_CELDA = 3,
        LIBERADO = 4,
        TRASLADADO = 5,
        SUJETO_A_PROCESO_EN_LIBERTAD = 6,
        DISCRECIONAL = 7,
        INDICIADOS = 8
    }

    public enum enumEstatusCausaPenal
    {
        POR_COMPURGAR = 0,       
        ACTIVO = 1,              
        SUSPENDIDO = 2,          
        SOBRESEIDO = 3,          
        CONCLUIDO = 4,           
        INACTIVO = 5,            
        EN_PROCESO = 6          
    }
}
