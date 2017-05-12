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


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cLiberacionAutoridad : EntityManagerServer<LIBERACION_AUTORIDAD>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cLiberacionAutoridad()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<LIBERACION_AUTORIDAD> ObtenerTodos(string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<LIBERACION_AUTORIDAD>();
                if (!string.IsNullOrEmpty(estatus))
                {
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                }
                return GetData(predicate.Expand());
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
        public IQueryable<LIBERACION_AUTORIDAD> Obtener(short? Id = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<LIBERACION_AUTORIDAD>();
                if (Id != null)
                    predicate = predicate.And(w => w.ID_LIBERACION_AUTORIDAD == Id);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

     
        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "FUERO" con valores a insertar</param>
        public short Insertar(LIBERACION_AUTORIDAD Entity)
        {
            try
            {
                Entity.ID_LIBERACION_AUTORIDAD = GetIDProceso<short>("LIBERACION_AUTORIDAD", "ID_LIBERACION_AUTORIDAD", "1 = 1");
                if(Insert(Entity))
                    return Entity.ID_LIBERACION_AUTORIDAD;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "FUERO" con valores a actualizar</param>
        public bool Actualizar(LIBERACION_AUTORIDAD Entity)
        {
            try
            {
                if(Update(Entity))
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
        public bool Eliminar(LIBERACION_AUTORIDAD Entity)
        {
            try
            {
                if (Delete(Entity))
                    return true;
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
            return false;
        }
        
    }
}