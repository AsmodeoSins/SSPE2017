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
    public class cTipoRecurso : EntityManagerServer<TIPO_RECURSO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTipoRecurso()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "TIPO_RECURSO"</returns>
        public IQueryable<TIPO_RECURSO> ObtenerTodos(string Tipo = "",string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<TIPO_RECURSO>();
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                if (!string.IsNullOrEmpty(Tipo))
                    predicate = predicate.And(x => x.TIPO == Tipo);
                return GetData(predicate.Expand()).OrderBy(w => w.ID_TIPO_RECURSO); 

                //if(string.IsNullOrEmpty(Tipo))
                //    return GetData().OrderBy(w => w.ID_TIPO_RECURSO);
                //else
                //    return GetData().Where(x => x.TIPO == Tipo).OrderBy(w => w.ID_TIPO_RECURSO);
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
        /// <returns>objeto de tipo "TIPO_RECURSO"</returns>
        public List<TIPO_RECURSO> Obtener(short Id)
        {
            var Resultado = new List<TIPO_RECURSO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_TIPO_RECURSO == Id).ToList();
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
        /// <param name="Entity">objeto de tipo "TIPO_RECURSO" con valores a insertar</param>
        public short Insertar(TIPO_RECURSO Entity)
        {
            try
            {
                //Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
                Insert(Entity);
                return Entity.ID_TIPO_RECURSO;
            }
            catch (Exception ex)
            {
                return 0;
                //throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "TIPO_RECURSO" con valores a actualizar</param>
        public bool Actualizar(TIPO_RECURSO Entity)
        {
            try
            {
                Update(Entity);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                //if (ex.Message.Contains("part of the object's key information"))
                //    throw new ApplicationException("La llave principal no se puede cambiar");
                //else
                //    throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(short Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_TIPO_RECURSO == Id).ToList();
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