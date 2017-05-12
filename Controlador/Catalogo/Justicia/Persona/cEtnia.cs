using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEtnia: EntityManagerServer<ETNIA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cEtnia()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<ETNIA> ObtenerTodos(string buscar="",string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<ETNIA>();
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                return GetData(predicate.Expand());
                //if (string.IsNullOrEmpty(buscar))
                //    return GetData();
                //else
                //    return GetData().Where(w => w.DESCR.Contains(buscar));
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public IQueryable<ETNIA> Obtener(int Id)
        {
            //var Resultado = new List<ETNIA>();
            try
            {
                return GetData().Where(w => w.ID_ETNIA == Id);
            }
            catch (Exception ex)
            {
                return null;
                //throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public void Insertar(ETNIA Entity)
        {
                try
                {
                    //Entity.ID_ETNIA = GetIDCatalogo<short>("ETNIA");
                    Entity.ID_ETNIA = GetSequence<short>("ETNIA_SEQ");
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
        public void Actualizar(ETNIA Entity)
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
                var Entity = GetData().Where(w => w.ID_ETNIA == Id).SingleOrDefault();
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

        public IEnumerable<cPoblacionIndigena> ObtenerPoblacionIndigena(short Centro) 
        {
            try
            {
                var query = new StringBuilder();
                query.Append("SELECT ");
                query.Append("E.ID_ETNIA, ");
                query.Append("E.DESCR ETNIA, ");
                //COMUN
                query.Append("COUNT(CASE WHEN I.ID_ESTATUS_ADMINISTRATIVO = 7 AND IM.SEXO = 'M' THEN 1 END) AS M_DISCRECIONAL, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO = 'C' AND IM.SEXO = 'F' AND N.ID_NUC IS NOT NULL THEN 1 END) AS F_IMPUTADO_COMUN, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO = 'C' AND IM.SEXO = 'M' AND N.ID_NUC IS NOT NULL THEN 1 END) AS M_IMPUTADO_COMUN, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO = 'C' AND IM.SEXO = 'F' AND N.ID_NUC IS NULL THEN 1 END) AS F_INDICIADO_COMUN, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO = 'C' AND IM.SEXO = 'M' AND N.ID_NUC IS NULL THEN 1 END) AS M_INDICIADO_COMUN, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO = 'C' AND IM.SEXO = 'F' THEN 1 END) AS F_PROCESADO_COMUN, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO = 'C' AND IM.SEXO = 'M' THEN 1 END) AS M_PROCESADO_COMUN, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '3' AND CP.CP_FUERO = 'C' AND IM.SEXO = 'F' THEN 1 END) AS F_SENTENCIADO_COMUN, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '3' AND CP.CP_FUERO = 'C' AND IM.SEXO = 'M' THEN 1 END) AS M_SENTENCIADO_COMUN, ");
                //FEDERAL
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO = 'F' AND IM.SEXO = 'F' AND N.ID_NUC IS NULL THEN 1 END) AS F_INDICIADO_FEDERAL, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO = 'F' AND IM.SEXO = 'M' AND N.ID_NUC IS NULL THEN 1 END) AS M_INDICIADO_FEDERAL, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO = 'F' AND IM.SEXO = 'F' THEN 1 END) AS F_PROCESADO_FEDERAL, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO = 'F' AND IM.SEXO = 'M' THEN 1 END) AS M_PROCESADO_FEDERAL, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '3' AND CP.CP_FUERO = 'F' AND IM.SEXO = 'F' THEN 1 END) AS F_SENTENCIADO_FEDERAL, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '3' AND CP.CP_FUERO = 'F' AND IM.SEXO = 'M' THEN 1 END) AS M_SENTENCIADO_FEDERAL, ");
                //SIN FUERO
                query.Append("COUNT(CASE WHEN I.ID_ESTATUS_ADMINISTRATIVO = 7 AND CP.CP_FUERO IS NULL THEN 1 END) AS DISCRECIONAL_SIN_FUERO, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO IS NULL AND IM.SEXO = 'F' AND N.ID_NUC IS NULL THEN 1 END) AS F_INDICIADO_SIN_FUERO, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO IS NULL AND IM.SEXO = 'M' AND N.ID_NUC IS NULL THEN 1 END) AS M_INDICIADO_SIN_FUERO, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO IS NULL AND IM.SEXO = 'F' THEN 1 END) AS F_PROCESADO_SIN_FUERO, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO IS NULL AND IM.SEXO = 'M' THEN 1 END) AS M_PROCESADO_SIN_FUERO ");
                query.Append("FROM SSP.INGRESO I ");
                query.Append("INNER JOIN SSP.IMPUTADO IM ON I.ID_CENTRO = IM.ID_CENTRO AND I.ID_ANIO = IM.ID_ANIO AND I.ID_IMPUTADO = IM.ID_IMPUTADO ");
                query.Append("LEFT JOIN SSP.CAUSA_PENAL CP ON I.ID_CENTRO = CP.ID_CENTRO AND I.ID_ANIO = CP.ID_ANIO AND I.ID_IMPUTADO = CP.ID_IMPUTADO AND I.ID_INGRESO = CP.ID_INGRESO AND CP.ID_ESTATUS_CP = 1 ");
                query.Append("LEFT JOIN SSP.NUC N ON CP.ID_CENTRO = N.ID_CENTRO AND CP.ID_ANIO = N.ID_ANIO AND CP.ID_IMPUTADO = N.ID_IMPUTADO AND CP.ID_INGRESO = N.ID_INGRESO AND CP.ID_CAUSA_PENAL = N.ID_CAUSA_PENAL ");
                query.Append("INNER JOIN SSP.ETNIA E ON IM.ID_ETNIA = E.ID_ETNIA ");
                query.AppendFormat("WHERE I.ID_UB_CENTRO = {0} AND I.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,7,8) AND IM.ID_ETNIA <> 9999  ",Centro);
                query.Append("GROUP BY E.ID_ETNIA,E.DESCR ");
                query.Append("ORDER BY E.DESCR ");
                return Context.Database.SqlQuery<cPoblacionIndigena>(query.ToString());

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }


    public class cPoblacionIndigena{
        public short ID_ETNIA{set; get;}
        public string ETNIA {set; get;}
        public short M_DISCRECIONAL{get;set;}
        public short F_IMPUTADO_COMUN{get;set;}
        public short M_IMPUTADO_COMUN{get;set;}
        public short F_INDICIADO_COMUN{get;set;}
        public short M_INDICIADO_COMUN{get;set;}
        public short F_PROCESADO_COMUN{get;set;}
        public short M_PROCESADO_COMUN{get;set;}
        public short F_SENTENCIADO_COMUN{get;set;}
        public short M_SENTENCIADO_COMUN{get;set;}
        public short F_INDICIADO_FEDERAL{get;set;}
        public short M_INDICIADO_FEDERAL{get;set;}
        public short F_PROCESADO_FEDERAL{get;set;}
        public short M_PROCESADO_FEDERAL{get;set;}
        public short F_SENTENCIADO_FEDERAL{get;set;}
        public short M_SENTENCIADO_FEDERAL{get;set;}
        public short DISCRECIONAL_SIN_FUERO{get;set;}
        public short F_INDICIADO_SIN_FUERO{get;set;}
        public short M_INDICIADO_SIN_FUERO{get;set;}
        public short F_PROCESADO_SIN_FUERO{get;set;}
        public short M_PROCESADO_SIN_FUERO{get;set;}
    }
   
}