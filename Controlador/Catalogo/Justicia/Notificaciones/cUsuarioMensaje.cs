using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;
using System.Data.Objects;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cUsuarioMensaje : EntityManagerServer<USUARIO_MENSAJE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cUsuarioMensaje()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "APODO"</returns>
       public IQueryable<USUARIO_MENSAJE> ObtenerTodos(string Usuario = "",string Buscar = "",DateTime? FechaInicio = null,DateTime? FechaFin = null,short Estatus = 1,short Order = 1,short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<USUARIO_MENSAJE>();
                predicate = predicate = predicate.And(w => w.ID_USUARIO.Trim() == Usuario);
                predicate = predicate = predicate.And(w => w.ESTATUS == Estatus);
                if (!string.IsNullOrEmpty(Buscar))
                    predicate = predicate.And(w => w.MENSAJE.ENCABEZADO.Contains(Buscar));
                if (FechaInicio != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.MENSAJE.REGISTRO_FEC) >= FechaInicio);
                if (FechaFin != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.MENSAJE.REGISTRO_FEC) <= FechaFin);
                if(Centro.HasValue)
                    predicate = predicate.And(w => w.MENSAJE.ID_CENTRO == Centro);
                
                if(Order == 1)//FECHA
                    return GetData(predicate.Expand()).OrderByDescending(w => w.MENSAJE.REGISTRO_FEC);
                else//PRIORIDAD
                    return GetData(predicate.Expand()).OrderBy(w => w.MENSAJE.MENSAJE_TIPO.PRIORIDAD);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


       public int ObtenerCount(string Usuario = "",DateTime? FechaInicio = null,DateTime? FechaFin = null)
       {
           try
           {
               var predicate = PredicateBuilder.True<USUARIO_MENSAJE>();
               predicate = predicate = predicate.And(w => w.ESTATUS == 1);
               if (FechaInicio != null)
                   predicate = predicate.And(w => w.MENSAJE.REGISTRO_FEC >= FechaInicio);
               if (FechaFin != null)
                   predicate = predicate.And(w => w.MENSAJE.REGISTRO_FEC <= FechaFin);
               return GetData(predicate.Expand()).Count();
               //return GetData().Where(w => w.ID_USUARIO.Trim() == Usuario && w.ESTATUS == 1).Count();
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
        /// <returns>objeto de tipo "APODO"</returns>
        public IQueryable<USUARIO_MENSAJE> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_MENSAJE == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ALIAS" con valores a insertar</param>
        public bool Insertar(USUARIO_MENSAJE Entity)
        {
            try
            {
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
        /// <param name="Entity">objeto de tipo "APODO" con valores a actualizar</param>
        public bool Actualizar(USUARIO_MENSAJE Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(List<USUARIO_MENSAJE> Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;
                else
                    return false;
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
        public bool Eliminar(string IdUsuario,int IdMensaje)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_USUARIO == IdUsuario && w.ID_MENSAJE == IdMensaje);//.SingleOrDefault();
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