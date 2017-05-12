using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cMediaFiliacion : EntityManagerServer<MEDIA_FILIACION>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cMediaFiliacion()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<MEDIA_FILIACION> ObtenerTodos(string buscar = "",string estatus = "S")
        {
            try
            {
                var predicate = PredicateBuilder.True<MEDIA_FILIACION>();
                if (!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                return GetData(predicate.Expand());
                //getDbSet();//Actualiza la informacion del GetData que se encuentra en memoria
                //if (string.IsNullOrEmpty(buscar))
                //    return GetData();
                //else
                //    return GetData().Where(w => w.DESCR.Contains(buscar));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// Buscar en la tipos de filiacion de la media filiacion
        /// </summary>
        /// <param name="buscar"></param>
        /// <returns></returns>
        public IQueryable<MEDIA_FILIACION> BuscarHijos(string buscar = "")
        {
            try
            {
                getDbSet();//Actualiza la informacion del GetData que se encuentra en memoria
                if (string.IsNullOrEmpty(buscar))
                    return GetData();
                else
                    return GetData().Where(w => w.TIPO_FILIACION.Where(x => x.DESCR.Contains(buscar)).Count() > 0);
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
        /// <returns>objeto de tipo "ETNIA"</returns>
        public IQueryable <MEDIA_FILIACION> Obtener(int Id)
        {
            try
            {
               return GetData().Where(w => w.ID_MEDIA_FILIACION == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public void Insertar(MEDIA_FILIACION Entity)
        {
            try
            {
                //TODO: TODO: AGREGAR MEDIA_FILIACION_SEQ (NO EXISTE)
                //Entity.ID_MEDIA_FILIACION = GetSequence<short>("MEDIA_FILIACION_SEQ");
                Entity.ID_MEDIA_FILIACION = GetIDCatalogo<short>("MEDIA_FILIACION");
                //Entity.ID_MEDIA_FILIACION = (short)(Entity.ID_MEDIA_FILIACION - 10);
                Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public void Actualizar(MEDIA_FILIACION Entity)
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
                var Entity = GetData().Where(w => w.ID_MEDIA_FILIACION == Id).SingleOrDefault();
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