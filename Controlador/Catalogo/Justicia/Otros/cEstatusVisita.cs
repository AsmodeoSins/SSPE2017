using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEstatusVisita : EntityManagerServer<ESTATUS_VISITA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cEstatusVisita()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "EJE"</returns>
        public List<ESTATUS_VISITA> ObtenerTodos(string buscar = "")
        {
            var Resultado = new List<ESTATUS_VISITA>();
            try
            {
                getDbSet();
                if (string.IsNullOrEmpty(buscar))
                    Resultado = GetData().OrderBy(x => x.ID_ESTATUS_VISITA).ToList();
                else
                    Resultado = GetData().Where(w => w.DESCR.Contains(buscar)).ToList();
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "TIPO_AUTORIDAD_INTERNA"</returns>
        public List<ESTATUS_VISITA> Obtener(string Id)
        {
            var Resultado = new List<ESTATUS_VISITA>();
            try
            {
                //Resultado = GetData().Where(w => w.ID_ESTATUS_VISITA == Id).ToList();
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
        /// <param name="Entity">objeto de tipo "TIPO_AUTORIDAD_INTERNA" con valores a insertar</param>
        public void Insertar(ESTATUS_VISITA Entity)
        {
            try
            {
                //Entity.ID_ESTATUS_VISITA = GetSequence<short>("ESTATUS_VISITA_SEQ").ToString();
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
        /// <param name="Entity">objeto de tipo "ESTATUS_VISITA" con valores a actualizar</param>
        public void Actualizar(ESTATUS_VISITA Entity)
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
        public bool Eliminar(string Id)
        {
            try
            {
                //var Entity = GetData().Where(w => w.ID_ESTATUS_VISITA == Id).SingleOrDefault();
                //if (Entity != null)
                //    return Delete(Entity);
                //else
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