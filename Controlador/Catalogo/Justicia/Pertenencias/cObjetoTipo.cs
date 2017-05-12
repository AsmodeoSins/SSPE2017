using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{

    public class cObjetoTipo : EntityManagerServer<OBJETO_TIPO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cObjetoTipo()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "OBJETO_TIPO"</returns>
        public IQueryable<OBJETO_TIPO> ObtenerTodos(string buscar = "", string permitido = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<OBJETO_TIPO>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                if (!string.IsNullOrEmpty(permitido))
                    predicate = predicate.And(w => w.PERMITIDO == permitido);
                return GetData(predicate.Expand()).OrderBy(w => w.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }



        public IQueryable<OBJETO_TIPO> ObtenerPara(string buscar = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<OBJETO_TIPO>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                return GetData(predicate.Expand()).OrderBy(w => w.DESCR);
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
        /// <returns>objeto de tipo "OBJETO_TIPO"</returns>
        public IQueryable<OBJETO_TIPO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_OBJETO_TIPO == Id);
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
        /// <returns>objeto de tipo "OBJETO_TIPO"</returns>
        public IQueryable<OBJETO_TIPO> ObtenerTodosTipos()
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
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "OBJETO_TIPO" con valores a insertar</param>
        public bool Insertar(OBJETO_TIPO Entity)
        {
            try
            {
                Entity.ID_OBJETO_TIPO = GetSequence<short>("OBJETO_TIPO_SEQ");
                if (Insert(Entity))
                    return true;
                else
                    return false;

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "OBJETO_TIPO" con valores a actualizar</param>
        public void Actualizar(OBJETO_TIPO Entity)
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
                var ListEntity = GetData().Where(w => w.ID_OBJETO_TIPO == Id);
                foreach (var entity in ListEntity)
                {
                    Delete(entity);
                }
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