using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using LinqKit;
using System.Transactions;
using System.Data;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEscalaRiesgo : EntityManagerServer<ESCALA_RIESGO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cEscalaRiesgo()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<ESCALA_RIESGO> ObtenerTodos(string NUC, string Paterno, string Materno, string Nombre, int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<ESCALA_RIESGO>();
                if (!string.IsNullOrEmpty(NUC))
                    predicate = predicate.And(w => w.NUC.Contains(NUC));
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.MATERNO.Contains(Materno));
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.NOMBRE.Contains(Nombre));
                return GetData(predicate.Expand()).OrderBy(w => w.NUC).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
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
        /// <returns>objeto de tipo "FUERO"</returns>
        public ESCALA_RIESGO Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_ESCALA_RIESGO == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESCALA_RIESGO" con valores a insertar</param>
        public int Insertar(ESCALA_RIESGO Entity)
        {
            try
            {
                Entity.ID_ESCALA_RIESGO = GetIDProceso<int>("ESCALA_RIESGO", "ID_ESCALA_RIESGO", "1=1");
                if (Insert(Entity))
                    return (int)Entity.ID_ESCALA_RIESGO;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "FUERO" con valores a actualizar</param>
        public bool Actualizar(ESCALA_RIESGO Entity)
        {
            try
            {
                return Update(Entity);
            }
            catch (Exception ex)
            {
               throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(ESCALA_RIESGO Entity)
        {
            try
            {
                    return Delete(Entity);
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
        
    }
}