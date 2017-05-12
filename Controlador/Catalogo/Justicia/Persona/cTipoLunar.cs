using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTipoLunar : EntityManagerServer<TIPO_LUNAR>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTipoLunar()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "TIPO_LUNAR"</returns>
        public List<TIPO_LUNAR> ObtenerTodos(string buscar = "")
        {
            List<TIPO_LUNAR> etnias;
            var Resultado = new List<TIPO_LUNAR>();
            try
            {
                if (string.IsNullOrEmpty(buscar))
                    Resultado = GetData().ToList();
                else
                    Resultado = GetData().Where(w => w.DESCR.Contains(buscar)).ToList();
                etnias = new List<TIPO_LUNAR>(Resultado);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return etnias;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "TIPO_LUNAR"</returns>
        public List<TIPO_LUNAR> Obtener(int Id)
        {
            var Resultado = new List<TIPO_LUNAR>();
            try
            {
                Resultado = GetData().Where(w => w.ID_LUNAR == Id).ToList();
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
        /// <param name="Entity">objeto de tipo "TIPO_LUNAR" con valores a insertar</param>
        public void Insertar(TIPO_LUNAR Entity)
        {
            try
            {
                Entity.ID_LUNAR = GetSequence<short>("TIPO_LUNAR_SEQ");
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
        /// <param name="Entity">objeto de tipo "TIPO_LUNAR" con valores a actualizar</param>
        public void Actualizar(TIPO_LUNAR Entity)
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
                var Entity = GetData().Where(w => w.ID_LUNAR == Id).SingleOrDefault();
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