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
    public class cAbogadoTitulo : EntityManagerServer<ABOGADO_TITULO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAbogadoTitulo()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        /// 
        public IQueryable<ABOGADO_TITULO> ObtenerTodos(string buscar = "", string clave = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<ABOGADO_TITULO>();
                if (!string.IsNullOrEmpty(clave))
                    predicate = predicate.And(w => w.ID_ABOGADO_TITULO.Contains(clave));

                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));

                return GetData(predicate.Expand()).OrderBy(w => w.ID_ABOGADO_TITULO);

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
        public IQueryable<ABOGADO_TITULO> Obtener(string Id)
        {
            try
            {
                return GetData().Where(w => w.ID_ABOGADO_TITULO == Id);
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

        public void Insertar(ABOGADO_TITULO Entity)
        {
            try
            {
                Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        //public void Actualizar(ABOGADO_TITULO entidad)
        //{
        //    try
        //    {
        //        Update(entidad);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
        //    }
        //}

        //public void Insertar(ABOGADO_TITULO Entity, SSPEntidades Contexto)
        //{
        //    try
        //    {
        //        Entity.ID_ABOGADO_TITULO = GetIDProceso<string>("ABOGADO_TITULO", "ID_ABOGADO_TITULO", "1 = 1", Contexto);
        //        if (Insert(Entity, Contexto))
        //            return Entity.ID_ABOGADO_TITULO;
        //        return "0";
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ApplicationException(ex.Message);
        //    }

        //}

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public bool Actualizar(ABOGADO_TITULO Entity)
        {
            try
            {
                return (Update(Entity));
                   
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
                var ListEntity = GetData().Where(w => w.ID_ABOGADO_TITULO == Id);
                if (ListEntity != null)
                {
                    foreach (var entity in ListEntity)
                    {
                        if (Delete(entity))
                            return true;
                    }
                    return false;
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
    }
}