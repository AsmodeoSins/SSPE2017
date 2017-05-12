using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTipoFiliacion : EntityManagerServer<TIPO_FILIACION>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTipoFiliacion()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<TIPO_FILIACION> ObtenerTodos(string Busqueda = "")
        {
            try
            {
                getDbSet();
                if(string.IsNullOrEmpty(Busqueda))
                    return GetData().OrderBy(x => x.ID_MEDIA_FILIACION);
                else
                    return GetData().Where(x => x.DESCR.Contains(Busqueda) || x.MEDIA_FILIACION.DESCR.Contains(Busqueda)).OrderBy(x => x.ID_MEDIA_FILIACION);
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
        public IQueryable <TIPO_FILIACION> Obtener(int Id,int media_filiacion)
        {
            try
            {
                return GetData().Where(w => w.ID_TIPO_FILIACION == Id && w.ID_MEDIA_FILIACION == media_filiacion);
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
        public TIPO_FILIACION Insertar(TIPO_FILIACION Entity)
        {
            try
            {
                //TODO: AGREGAR TIPO_FILIACION_SEQ (NO EXISTE)
               // Entity.ID_TIPO_FILIACION = GetSequence<short>("TIPO_FILIACION_SEQ");
                Entity.ID_TIPO_FILIACION = GetIDCatalogo<short>("TIPO_FILIACION");
                Insert(Entity);
                return Entity;
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
        public void Actualizar(TIPO_FILIACION Entity)
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
                var Entity = GetData().Where(w => w.ID_TIPO_FILIACION == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch(Exception ex)
            {
                if (ex.InnerException != null)
                {
                    //if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                    //    throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                    throw new ApplicationException("Error: No se puede eliminar Tipo Filiacion: Tiene dependencias.");
                }
            }
            return false;
        }
    }
}