using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cDecomisoObjeto : EntityManagerServer<DECOMISO_OBJETO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cDecomisoObjeto()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO_OBJETO> ObtenerTodos(short? Tipo = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<DECOMISO_OBJETO>();
                if (Tipo.HasValue)
                    predicate = predicate.And(w => w.ID_OBJETO_TIPO == Tipo);
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
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO_OBJETO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_DECOMISO == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO_OBJETO> ObtenerObjetos(int? ID_OBJETO_TIPO = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<DECOMISO_OBJETO>();
                if (ID_OBJETO_TIPO.HasValue)
                    predicate = predicate.And(a => a.ID_OBJETO_TIPO == ID_OBJETO_TIPO);
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
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a insertar</param>
        public short Insertar(DECOMISO_OBJETO Entity)
        {
            try
            {
                Entity.ID_CONSEC = GetIDProceso<short>("DECOMISO_OBJETO", "ID_CONSEC", string.Format("ID_DECOMISO = {0}", Entity.ID_DECOMISO));
                if (Insert(Entity))
                    return Entity.ID_CONSEC;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }

        public bool Insertar(List<DECOMISO_OBJETO> Entity, int Id)
        {
            try
            {
                if (new cDecomisoImagen().Eliminar(Id))
                    if (new cDecomisoObjeto().Eliminar(Id))
                    {
                        if (Entity.Count == 0)
                            return true;
                        if (Insert(Entity))
                            return true;
                    }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }







        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public bool Actualizar(DECOMISO_OBJETO Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;
            }
            catch (Exception ex)
            {
                //if (ex.Message.Contains("part of the object's key information"))
                //    throw new ApplicationException("La llave principal no se puede cambiar");
                //else
                //    throw new ApplicationException(ex.Message);
                return false;
            }
            return false;
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
                var ListEntity = GetData().Where(w => w.ID_DECOMISO == Id);
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