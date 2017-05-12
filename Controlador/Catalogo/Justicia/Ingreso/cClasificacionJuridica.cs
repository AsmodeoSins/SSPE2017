using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cClasificacionJuridica : EntityManagerServer<CLASIFICACION_JURIDICA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cClasificacionJuridica()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "CLASIFICACION_JURIDICA"</returns>
        public IQueryable<CLASIFICACION_JURIDICA> ObtenerTodos(string buscar = "",string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<CLASIFICACION_JURIDICA>();
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                return GetData(predicate.Expand()).OrderBy(x => x.ID_CLASIFICACION_JURIDICA);


                //if (string.IsNullOrEmpty(buscar))
                //    return GetData().Where(w => w.ESTATUS == estatus).OrderBy(x => x.ID_CLASIFICACION_JURIDICA);
                //else
                //    return GetData().Where(w => w.DESCR.Contains(buscar) && w.ESTATUS == estatus).OrderBy(x => x.ID_CLASIFICACION_JURIDICA); 
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
        /// <returns>objeto de tipo "CLASIFICACION_JURIDICA"</returns>
        public IQueryable<CLASIFICACION_JURIDICA> Obtener(string Id)
        {
            try
            {
                return GetData().Where(w => w.ID_CLASIFICACION_JURIDICA == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "CLASIFICACION_JURIDICA" con valores a insertar</param>
        public bool Insertar(CLASIFICACION_JURIDICA Entity)
        {
            try
            {
                //Entity.ID_ESTADO_CIVIL = GetSequence<short>("ESTADO_CIVIL_SEQ");
                return Insert(Entity);
                
            }
            catch (Exception ex)
            {                
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "CLASIFICACION_JURIDICA" con valores a actualizar</param>
        public bool Actualizar(CLASIFICACION_JURIDICA Entity)
        {
            try
            {
                return Update(Entity);
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
        public bool Eliminar(string Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CLASIFICACION_JURIDICA == Id).SingleOrDefault();
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
    }
}