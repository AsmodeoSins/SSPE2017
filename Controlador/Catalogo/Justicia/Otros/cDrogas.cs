using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cDrogas : EntityManagerServer<DROGA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cDrogas()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "TIPO_AUTORIDAD_INTERNA"</returns>
        public IQueryable<DROGA> ObtenerTodos(string buscar = "")
        {
            var Resultado = new List<DROGA>();
            try
            {
                getDbSet();
                if (string.IsNullOrEmpty(buscar))
                    return GetData().OrderBy(x => x.ID_DROGA);
                else
                    return GetData().Where(w => w.DESCR.Contains(buscar));
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "TIPO_AUTORIDAD_INTERNA"</returns>
        public DROGA Obtener(int Id)
        {
            return GetData().Where(w => w.ID_DROGA == Id).FirstOrDefault();
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "TIPO_AUTORIDAD_INTERNA" con valores a insertar</param>
        public void Insertar(DROGA Entity)
        {
            try
            {
                Entity.ID_DROGA = GetSequence<short>("DROGA_SEQ");
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
        /// <param name="Entity">objeto de tipo "TIPO_AUTORIDAD_INTERNA" con valores a actualizar</param>
        public void Actualizar(DROGA Entity)
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
                var Entity = GetData().Where(w => w.ID_DROGA == Id).SingleOrDefault();
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