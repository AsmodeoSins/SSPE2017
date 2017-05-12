using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPaises : EntityManagerServer<PAIS_NACIONALIDAD>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cPaises()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PAIS_NACIONALIDAD"</returns>
        public IQueryable<PAIS_NACIONALIDAD> ObtenerTodos(string buscar = "",string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<PAIS_NACIONALIDAD>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.PAIS.Contains(buscar) || w.NACIONALIDAD.Contains(buscar));
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand()).OrderBy(x => x.PAIS);
                //if (string.IsNullOrEmpty(buscar))
                //    return GetData().OrderBy(x => x.PAIS);
                //else
                //    return GetData().Where(w => w.PAIS.Contains(buscar) || w.NACIONALIDAD.Contains(buscar)).OrderBy(x => x.PAIS);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public IQueryable<PAIS_NACIONALIDAD> ObtenerNacionalidad(string buscar = "",string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<PAIS_NACIONALIDAD>();
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                if (!string.IsNullOrEmpty(buscar))
                { 
                    predicate = predicate.And(w => (w.PAIS.Contains(buscar) || w.NACIONALIDAD.Contains(buscar)) && (w.NACIONALIDAD != "" || w.NACIONALIDAD != null));
                    return GetData(predicate.Expand()).OrderBy(x => x.NACIONALIDAD); ;
                }
                else
                    return GetData(predicate.Expand()).OrderBy(x => x.PAIS); 
                //if (string.IsNullOrEmpty(buscar))
                //    return GetData().Where(w => w.NACIONALIDAD != "" || w.NACIONALIDAD != null).OrderBy(x => x.PAIS);
                //else
                //    return GetData().Where(w => (w.PAIS.Contains(buscar) || w.NACIONALIDAD.Contains(buscar)) && (w.NACIONALIDAD != "" || w.NACIONALIDAD != null)).OrderBy(x => x.NACIONALIDAD);
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
        public IQueryable<PAIS_NACIONALIDAD> Obtener(int Id)
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
        public void Insertar(PAIS_NACIONALIDAD Entity)
        {
            try
            {
                Entity.ID_PAIS_NAC = GetSequence<short>("PAIS_NACIONALIDAD_SEQ");
                //Entity.ID_PAIS_NAC = GetIDCatalogo<short>("PAIS_NACIONALIDAD");
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
        public void Actualizar(PAIS_NACIONALIDAD Entity)
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
                var Entity = GetData().Where(w => w.ID_PAIS_NAC == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("child record found"))
                {
                    //if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                    //    throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                    return false;
                }
            }
            return false;
        }


        public IEnumerable<cPoblacionActivaCierre> ObtenerPoblacionActivaCierre(int Centro, DateTime? Fechainicio = null, DateTime? FechaFin = null)
        {
            try
            {
                string query = "SELECT P.PAIS, "+
                "COUNT(CASE WHEN I.NACIMIENTO_PAIS = P.ID_PAIS_NAC AND I.SEXO = 'F'THEN 1 END) FEMENINO,  "+
                "COUNT(CASE WHEN I.NACIMIENTO_PAIS = P.ID_PAIS_NAC AND I.SEXO = 'M' THEN 1 END) MASCULINO,  "+
                "COUNT(CASE WHEN I.NACIMIENTO_PAIS = P.ID_PAIS_NAC  THEN 1 END) TOTAL "+
                "FROM ( "+
                "SELECT ID_CENTRO,ID_ANIO,ID_IMPUTADO,MAX(ID_INGRESO) ID_INGRESO FROM SSP.INGRESO "+
                "WHERE ID_UB_CENTRO = {0}   "+
                "GROUP BY ID_CENTRO,ID_ANIO,ID_IMPUTADO "+
                ") TMP "+
                "INNER JOIN SSP.INGRESO ING ON TMP.ID_CENTRO = ING.ID_CENTRO AND TMP.ID_ANIO = ING.ID_ANIO AND TMP.ID_IMPUTADO = ING.ID_IMPUTADO AND TMP.ID_INGRESO = ING.ID_INGRESO "+
                "INNER JOIN SSP.IMPUTADO I ON ING.ID_CENTRO = I.ID_CENTRO AND ING.ID_ANIO = I.ID_ANIO AND ING.ID_IMPUTADO = I.ID_IMPUTADO  "+
                "INNER JOIN SSP.PAIS_NACIONALIDAD P ON I.NACIMIENTO_PAIS = P.ID_PAIS_NAC "+
                "WHERE (ING.FEC_SALIDA_CERESO IS NULL OR (TRUNC(ING.FEC_SALIDA_CERESO) >= '{1}' AND TRUNC(ING.FEC_SALIDA_CERESO) <= '{2}')) GROUP BY P.PAIS ORDER BY P.PAIS";
                query = string.Format(query, Centro, Fechainicio.Value.ToShortDateString(), FechaFin.Value.ToShortDateString());
                return Context.Database.SqlQuery<cPoblacionActivaCierre>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cPoblacionExtranjera> ObtenerPoblacionExtranjera(short Centro) 
        {
            try
            {
                var query = new StringBuilder();
                query.Append("SELECT * FROM ");
                query.Append("( ");
                query.Append("SELECT  ");
                query.Append("P.ID_PAIS_NAC, ");
                query.Append("P.PAIS, ");
                //COMUN
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO = 'C' AND N.ID_NUC IS NULL THEN 1 END) AS PROCESADO_COMUN, ");
                //SIN FUERO
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' AND CP.CP_FUERO IS NULL AND N.ID_NUC IS NULL THEN 1 END) AS INDICIADO_SIN_FUERO, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '2' AND CP.CP_FUERO IS NULL AND N.ID_NUC IS NULL THEN 1 END) AS PROCESADO_SIN_FUERO, ");
                query.Append("COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '3' AND CP.CP_FUERO IS NULL AND N.ID_NUC IS NULL THEN 1 END) AS SENTENCIADO_SIN_FUERO ");
                query.Append("FROM SSP.INGRESO I ");
                query.Append("INNER JOIN SSP.IMPUTADO IM ON I.ID_CENTRO = IM.ID_CENTRO AND I.ID_ANIO = IM.ID_ANIO AND I.ID_IMPUTADO = IM.ID_IMPUTADO ");
                query.Append("LEFT JOIN SSP.CAUSA_PENAL CP ON I.ID_CENTRO = CP.ID_CENTRO AND I.ID_ANIO = CP.ID_ANIO AND I.ID_IMPUTADO = CP.ID_IMPUTADO AND I.ID_INGRESO = CP.ID_INGRESO AND CP.ID_ESTATUS_CP = 1 ");
                query.Append("LEFT JOIN SSP.NUC N ON CP.ID_CENTRO = N.ID_CENTRO AND CP.ID_ANIO = N.ID_ANIO AND CP.ID_IMPUTADO = N.ID_IMPUTADO AND CP.ID_INGRESO = N.ID_INGRESO AND CP.ID_CAUSA_PENAL = N.ID_CAUSA_PENAL ");
                query.Append("INNER JOIN SSP.PAIS_NACIONALIDAD P ON IM.NACIMIENTO_PAIS = P.ID_PAIS_NAC ");
                query.AppendFormat("WHERE I.ID_UB_CENTRO = {0} AND I.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) AND IM.NACIMIENTO_PAIS <> 82 ",Centro);
                query.Append("GROUP BY P.ID_PAIS_NAC,P.PAIS ");
                query.Append("ORDER BY P.PAIS ");
                query.Append(") ");
                query.Append("WHERE PROCESADO_COMUN > 0 OR INDICIADO_SIN_FUERO > 0 OR PROCESADO_SIN_FUERO > 0 OR SENTENCIADO_SIN_FUERO > 0 ");
                return Context.Database.SqlQuery<cPoblacionExtranjera>(query.ToString());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }

    public class cPoblacionActivaCierre {
        public string PAIS { get; set; }
        public int FEMENINO { get; set; }
        public int MASCULINO { get; set; }
        public int TOTAL { get; set; }
    }

    public class cPoblacionExtranjera
    {
        public short ID_PAIS_NAC { get; set; }
        public string PAIS { get; set; }
        public short PROCESADO_COMUN { get; set; }
        public short INDICIADO_SIN_FUERO { get; set; }
        public short PROCESADO_SIN_FUERO { get; set; }
        public short SENTENCIADO_SIN_FUERO { get; set; }
       
    }

}