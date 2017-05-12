using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cColonia : EntityManagerServer<COLONIA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cColonia()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<COLONIA> ObtenerTodos(string buscar = "", short? municipio = 0)
        {
            try
            {
                getDbSet();

                if (string.IsNullOrEmpty(buscar))
                {
                    return GetData().Where(w => w.ID_MUNICIPIO == municipio);
                }
                else
                {
                    if (municipio == 0)
                    {
                        return GetData().Where(w => w.DESCR.Contains(buscar));
                    }
                    else
                    {
                        return GetData().Where(w => w.ID_MUNICIPIO == municipio && w.DESCR.Contains(buscar));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public IQueryable<COLONIA> ObtenerTodos(string buscar = "", short? municipio = 0, short? entidad = 0)
        {
            try
            {
                getDbSet();

                return GetData().Where(w => (string.IsNullOrEmpty(buscar) ? true : w.DESCR.Contains(buscar)) && 
                    (municipio.HasValue ? municipio > 0 ? w.ID_MUNICIPIO == municipio : true : true) &&
                    (entidad.HasValue ? entidad > 0 ? w.ID_ENTIDAD == entidad : true : true));
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
        public IQueryable<COLONIA> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_COLONIA == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "COLONIA" con valores a insertar</param>
        public void Insertar(COLONIA Entity)
        {
            try
            {
                //Entity.ID_COLONIA = GetIDCatalogo<short>("COLONIA");
                Entity.ID_COLONIA = GetSequence<short>("COLONIA_SEQ");
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
        /// <param name="Entity">objeto de tipo "COLONIA" con valores a actualizar</param>
        public void Actualizar(COLONIA Entity)
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
                var Entity = GetData().Where(w => w.ID_COLONIA == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {

                if(ex.Message.Contains("child record found")){
                    return false;
                }
                //if (ex.InnerException != null)
                //{
                //    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                //        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                //}
               // throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            return false;
        }
    }
}