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
    public class cTipoDelito : EntityManagerServer<TIPO_DELITO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTipoDelito()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "TIPO_DELITO"</returns>
        public IQueryable<TIPO_DELITO> ObtenerTodos(string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<TIPO_DELITO>();
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
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
        /// <returns>objeto de tipo "TIPO_DELITO"</returns>
        public List<TIPO_DELITO> Obtener(int TipoDelito)
        {
            var Resultado = new List<TIPO_DELITO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_TIPO_DELITO == TipoDelito).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "TIPO_DELITO" con valores a insertar</param>
        public void Insertar(TIPO_DELITO Entity)
        {
            try
            {
                Entity.ID_TIPO_DELITO = GetSequence<short>("TIPO_DELITO_SEQ");
                Insert(Entity);
                //return  Entity.ID_TIPO_DELITO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            //return 0;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "TIPO_DELITO" con valores a actualizar</param>
        public void Actualizar(TIPO_DELITO Entity)
        {
            try
            {
                Update(Entity);
                //return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
            //return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int TipoDelito)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_TIPO_DELITO == TipoDelito).ToList();
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