using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public  class cJuzgadoTipo : EntityManagerServer<TIPO_JUZGADO>
    {

        public IQueryable<TIPO_JUZGADO> Obtener()
        {
            try
            {

                return GetData();

            }
            catch (Exception ex)
            {
                return null;
                //throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "JUZGADO"</returns>
        public IQueryable<TIPO_JUZGADO> ObtenerTodos()
        {
            try
            {

                return GetData();
                
            }
            catch (Exception ex)
            {
                return null;
                //throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "juzgado"</returns>
        public List<TIPO_JUZGADO> Obtener(short id)
        {
            try
            {
                return GetData().Where(w => w.ID_TIPO_JUZGADO== id).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "JUZGADO" con valores a insertar</param>
        public long Insertar(TIPO_JUZGADO Entity)
        {
            try
            {
                Entity.ID_TIPO_JUZGADO= GetSequence<short>("TIPO_JUZGADO_SEQ");
                Insert(Entity);
                return Entity.ID_TIPO_JUZGADO;
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
        /// <param name="Entity">objeto de tipo "JUZGADO" con valores a actualizar</param>
        public bool Actualizar(TIPO_JUZGADO Entity)
        {
            try
            {
                Update(Entity);
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
        public bool EliminarObtener(TIPO_JUZGADO Entity)
        {
            try
            {
                    return Delete(Entity);
             
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
