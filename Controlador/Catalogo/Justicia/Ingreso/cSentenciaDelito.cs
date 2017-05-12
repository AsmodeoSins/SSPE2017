using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSentenciaDelito : EntityManagerServer<SENTENCIA_DELITO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cSentenciaDelito()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "SENTENCIA_DELITO"</returns>
        public IQueryable<SENTENCIA_DELITO> ObtenerTodos()
        {
            try
            {
                return GetData();
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
        /// <returns>objeto de tipo "SENTENCIA_DELITO"</returns>
        public IQueryable<SENTENCIA_DELITO> ObtenerTodos(short Centro = 0, short Anio = 0, int Imputado = 0, short Ingreso = 0, short CausaPenal = 0, short Sentencia = 0)
        {
            try
            {
               return GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_SENTENCIA == Sentencia);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "SENTENCIA_DELITO" con valores a insertar</param>
        public int Insertar(SENTENCIA_DELITO Entity)
        {
            try
            {
                //Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
                Insert(Entity);
                return Entity.ID_IMPUTADO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "CAUSA_PENAL_DELITO" con valores a insertar</param>
        public bool Insertar(short Centro, short Anio, int Imputado, short Ingreso, short CausaPenal,short Sentencia, List<SENTENCIA_DELITO> Delitos)
        {
            try
            {
                Eliminar(Centro,Anio,Imputado,Ingreso,CausaPenal,Sentencia);
                if (Delitos.Count == 0)
                    return true;
                else
                    if (Insert(Delitos))
                        return true;
            }
            catch (Exception ex)
            {
                return false;
                //throw new ApplicationException(ex.Message);
            }
            return false;
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "SENTENCIA_DELITO" con valores a actualizar</param>
        public bool Actualizar(SENTENCIA_DELITO Entity)
        {
            try
            {
                Update(Entity);
                return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal, int Sentencia)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_SENTENCIA ==  Sentencia).ToList();
                if (Entity != null)
                {
                    foreach (SENTENCIA_DELITO delito in Entity)
                    {
                        Delete(delito);
                    }
                    return true;
                }
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

        public IEnumerable<cPoblacionDelito> ObtenerPoblacionDelito(short Centro, string Fuero)
        {
            try
            {
                string query = "SELECT D.ID_DELITO,D.DESCR AS DELITO, " +
                                "IM.SEXO, " +
                                "COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '1' OR I.ID_CLASIFICACION_JURIDICA = 'I' THEN 1 END) INDICIADO, " +
                                "COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '2' OR I.ID_CLASIFICACION_JURIDICA = 'P' THEN 1 END) PROCESADO, " +
                                "COUNT(CASE WHEN I.ID_CLASIFICACION_JURIDICA = '3' OR I.ID_CLASIFICACION_JURIDICA = 'S' THEN 1 END) SENTENCIADO, " +
                                "COUNT(SD.ID_DELITO) TOTAL " +
                                "FROM SSP.SENTENCIA_DELITO SD " +
                                "INNER JOIN SSP.CAUSA_PENAL CP ON SD.ID_CENTRO = CP.ID_CENTRO AND SD.ID_ANIO = CP.ID_ANIO AND SD.ID_IMPUTADO = CP.ID_IMPUTADO AND SD.ID_INGRESO = CP.ID_INGRESO AND SD.ID_CAUSA_PENAL = CP.ID_CAUSA_PENAL " +
                                "INNER JOIN SSP.SENTENCIA S ON CP.ID_CENTRO = S.ID_CENTRO AND CP.ID_ANIO = S.ID_ANIO AND CP.ID_IMPUTADO = S.ID_IMPUTADO AND CP.ID_INGRESO = S.ID_INGRESO AND CP.ID_CAUSA_PENAL = S.ID_CAUSA_PENAL " +
                                "INNER JOIN SSP.INGRESO I ON CP.ID_CENTRO = I.ID_CENTRO AND CP.ID_ANIO = I.ID_ANIO AND CP.ID_IMPUTADO = I.ID_IMPUTADO AND CP.ID_INGRESO = I.ID_INGRESO  " +
                                "INNER JOIN SSP.CLASIFICACION_JURIDICA CJ ON I.ID_CLASIFICACION_JURIDICA = CJ.ID_CLASIFICACION_JURIDICA " +
                                "INNER JOIN SSP.IMPUTADO IM ON I.ID_CENTRO = IM.ID_CENTRO AND I.ID_ANIO = IM.ID_ANIO AND I.ID_IMPUTADO = IM.ID_IMPUTADO " +
                                "INNER JOIN SSP.DELITO D ON SD.ID_DELITO = D.ID_DELITO " +
                                "WHERE I.ID_UB_CENTRO = {0} AND D.ID_FUERO = '{1}' AND I.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) AND S.ESTATUS = 'A' " +
                                "GROUP BY D.ID_DELITO,D.DESCR,IM.SEXO ";
                query = string.Format(query, Centro, Fuero);
                return Context.Database.SqlQuery<cPoblacionDelito>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        
    }

    public class cPoblacionDelito
    {
        public long ID_DELITO { get; set; }
        public string DELITO { get; set; }
        public string SEXO { get; set; }
        public int INDICIADO { get; set; }
        public int PROCESADO { get; set; }
        public int SENTENCIADO { get; set; }
        public int TOTAL { get; set; }
    }
}