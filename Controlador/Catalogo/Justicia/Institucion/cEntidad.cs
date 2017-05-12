using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEntidad : EntityManagerServer<ENTIDAD>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cEntidad()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// <param name="pais">id del pais a filtrar</param>
        /// /// <param name="buscar_estado">nombre del estado a filtrar(opcional)</param>
        /// </summary>
        /// <returns>listado de tipo "ENTIDAD"</returns>
        public IQueryable<ENTIDAD> ObtenerTodosPais(short pais,string buscar_estado = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<ENTIDAD>();
                predicate = predicate.And(w => w.ID_PAIS_NAC == pais);
                if (!string.IsNullOrEmpty(buscar_estado))
                    predicate.And(w => w.DESCR.Contains(buscar_estado));
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PAIS_NACIONALIDAD"</returns>
        public IQueryable<ENTIDAD> ObtenerTodos(string buscar = "",string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<ENTIDAD>();
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                return GetData(predicate.Expand());
                //if (string.IsNullOrEmpty(buscar_estado))
                //    return GetData();
                //else
                //    return GetData().Where(w => w.DESCR.Contains(buscar_estado));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public IQueryable<ENTIDAD> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_PAIS_NAC == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public void Insertar(ENTIDAD Entity)
        {
            try
            {
                //Entity.ID_PAIS_NAC = GetIDCatalogo<short>("ENTIDAD");
                Entity.ID_ENTIDAD = GetSequence<short>("ENTIDAD_SEQ");
                Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public void Actualizar(ENTIDAD Entity)
        {
            try
            {
                Update(Entity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ENTIDAD == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IEnumerable<cPoblacionProcedencia> ObtenerPoblacionPorEntidad(int Centro) 
        {
            try
            {
                string query = "SELECT "+
                "E.ID_ENTIDAD, "+
                "E.DESCR AS ENTIDAD, "+
                /*FUERO COMUN MASCULINO*/
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('1','I') AND CP.CP_FUERO = 'C' AND I.SEXO = 'M' AND N.ID_NUC IS NULL  THEN 1 END) INDICIADO_COMUN_M, "+ 
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('1','I') AND CP.CP_FUERO = 'C' AND I.SEXO = 'M' AND N.ID_NUC IS NOT NULL  THEN 1 END) IMPUTADO_COMUN_M, "+ 
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('2','P')  AND CP.CP_FUERO = 'C' AND I.SEXO = 'M' THEN 1 END) PROCESADO_COMUM_M, "+ 
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('3','S') AND CP.CP_FUERO = 'C' AND I.SEXO = 'M' THEN 1 END) SENTENCIADO_COMUN_M, "+ 
                "COUNT(CASE WHEN (ING.ID_CLASIFICACION_JURIDICA = '4') AND CP.CP_FUERO = 'C' AND I.SEXO = 'M' THEN 1 END) DISCRECIONAL_COMUN_M, "+
                /*FUERO COMUN FEMENINO*/
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('1','I') AND CP.CP_FUERO = 'C' AND I.SEXO = 'F' AND N.ID_NUC IS NULL  THEN 1 END) INDICIADO_COMUN_F, " +
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('1','I') AND CP.CP_FUERO = 'C' AND I.SEXO = 'F' AND N.ID_NUC IS NOT NULL  THEN 1 END) IMPUTADO_COMUN_F, " + 
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('2','P') AND CP.CP_FUERO = 'C' AND I.SEXO = 'F' THEN 1 END) PROCESADO_COMUM_F, "+ 
                "COUNT(CASE WHEN (ING.ID_CLASIFICACION_JURIDICA = '3' OR ING.ID_CLASIFICACION_JURIDICA = 'S') AND CP.CP_FUERO = 'C' AND I.SEXO = 'F' THEN 1 END) SENTENCIADO_COMUN_F, "+ 
                /*FUERO FEDERAL MASCULINO*/
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('1','I') AND CP.CP_FUERO = 'F' AND I.SEXO = 'M'   THEN 1 END) INDICIADO_FEDERAL_M, "+ 
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('2','P')  AND CP.CP_FUERO = 'F' AND I.SEXO = 'M'  THEN 1 END) PROCESADO_FEDERAL_M, "+ 
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('3','S') AND CP.CP_FUERO = 'F' AND I.SEXO = 'M'  THEN 1 END) SENTENCIADO_FEDERAL_M, "+ 
                /*FUERO FEDERAL FEMENINO*/
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('1','I') AND CP.CP_FUERO = 'F' AND I.SEXO = 'F' THEN 1 END) INDICIADO_FEDERAL_F, "+ 
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('2','P')  AND CP.CP_FUERO = 'F' AND I.SEXO = 'F' THEN 1 END) PROCESADO_FEDERAL_F, "+ 
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('3','S') AND CP.CP_FUERO = 'F' AND I.SEXO = 'F' THEN 1 END) SENTENCIADO_FEDERAL_F, "+ 
                /*SIN FUERO DISCRECIONAL*/
                "COUNT(CASE WHEN (ING.ID_CLASIFICACION_JURIDICA = '4') AND (CP.CP_FUERO IS NULL) THEN 1 END) DISCRECIONAL_SIN_FUERO, " +
                /*SIN FUERO MASCULINO*/
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('1','I') AND (CP.CP_FUERO IS NULL) AND I.SEXO = 'M' AND N.ID_NUC IS NULL THEN 1 END) INDICIADO_SIN_FUERO_M, " +
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('2','P')  AND (CP.CP_FUERO IS NULL) AND I.SEXO = 'M' THEN 1 END) PROCESADO_SIN_FUERO_M, " + 
                /*SIN FUERO FEMENINO*/
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('1','I') AND (CP.CP_FUERO IS NULL) AND I.SEXO = 'F' AND N.ID_NUC IS NULL THEN 1 END) INDICIADO_SIN_FUERO_F, " +
                "COUNT(CASE WHEN ING.ID_CLASIFICACION_JURIDICA IN ('2','P')  AND (CP.CP_FUERO IS NULL) AND I.SEXO = 'F' THEN 1 END) PROCESADO_SIN_FUERO_F, " + 
                /*TOTAL*/
                "COUNT(CASE WHEN (CP.CP_FUERO IS NOT NULL OR (CP.CP_FUERO IS NULL AND ING.ID_CLASIFICACION_JURIDICA IN ('1','2','I','P'))) THEN 1 END) TOTAL " +
                "FROM SSP.ENTIDAD E "+
                "LEFT JOIN SSP.IMPUTADO I ON E.ID_PAIS_NAC = I.NACIMIENTO_PAIS AND E.ID_ENTIDAD = I.NACIMIENTO_ESTADO "+
                "LEFT JOIN SSP.INGRESO ING ON I.ID_CENTRO = ING.ID_CENTRO AND I.ID_ANIO = ING.ID_ANIO AND I.ID_IMPUTADO = ING.ID_IMPUTADO AND ING.ID_UB_CENTRO = {0} AND ING.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) "+
                "LEFT JOIN SSP.CAUSA_PENAL CP ON ING.ID_CENTRO = CP.ID_CENTRO AND ING.ID_ANIO = CP.ID_ANIO AND ING.ID_IMPUTADO = CP.ID_IMPUTADO AND ING.ID_INGRESO = CP.ID_INGRESO AND CP.ID_ESTATUS_CP = 1" +
                "LEFT JOIN SSP.NUC N ON CP.ID_CENTRO = N.ID_CENTRO AND CP.ID_ANIO = N.ID_ANIO AND CP.ID_IMPUTADO = N.ID_IMPUTADO AND CP.ID_INGRESO = N.ID_INGRESO AND CP.ID_CAUSA_PENAL = N.ID_CAUSA_PENAL "+
                "WHERE E.ID_PAIS_NAC = 82 AND E.ESTATUS = 'S' " +
                "GROUP BY E.ID_ENTIDAD,E.DESCR "+
                "ORDER BY E.DESCR ";
                query = string.Format(query, Centro);
                return Context.Database.SqlQuery<cPoblacionProcedencia>(query);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }

    public class cPoblacionProcedencia
    {
        public int ID_ENTIDAD { get; set; }
        public string ENTIDAD { get; set; }
        public int INDICIADO_COMUN_M { get; set; }
        public int IMPUTADO_COMUN_M { get; set; }
        public int PROCESADO_COMUN_M { get; set; }
        public int SENTENCIADO_COMUN_M { get; set; }
        public int DISCRECIONAL_COMUN_M { get; set; }
        public int INDICIADO_COMUN_F { get; set; }
        public int IMPUTADO_COMUN_F { get; set; }
        public int PROCESADO_COMUN_F { get; set; }
        public int SENTENCIADO_COMUN_F { get; set; }
        public int INDICIADO_FEDERAL_M { get; set; }
        public int PROCESADO_FEDERAL_M { get; set; }
        public int SENTENCIADO_FEDERAL_M { get; set; }
        public int INDICIADO_FEDERAL_F { get; set; }
        public int PROCESADO_FEDERAL_F { get; set; }
        public int SENTENCIADO_FEDERAL_F { get; set; }
        public int DISCRECIONAL_SIN_FUERO { get; set; }
        public int INDICIADO_SIN_FUERO_M { get; set; }
        public int PROCESADO_SIN_FUERO_M { get; set; }
        public int INDICIADO_SIN_FUERO_F { get; set; }
        public int PROCESADO_SIN_FUERO_F { get; set; }
        public int TOTAL { get; set; }
    }
}