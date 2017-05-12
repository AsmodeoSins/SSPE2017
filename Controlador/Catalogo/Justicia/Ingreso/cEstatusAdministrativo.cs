using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEstatusAdministrativo : EntityManagerServer<ESTATUS_ADMINISTRATIVO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cEstatusAdministrativo()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ESTATUS_ADMINISTRATIVO"</returns>
        public IQueryable<ESTATUS_ADMINISTRATIVO> ObtenerTodos(string buscar = "")
        {
            try
            {
                if (string.IsNullOrEmpty(buscar))
                    return GetData();
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
        /// <returns>objeto de tipo "ESTATUS_ADMINISTRATIVO"</returns>
        public List<ESTATUS_ADMINISTRATIVO> Obtener(int Id)
        {
            var Resultado = new List<ESTATUS_ADMINISTRATIVO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_ESTATUS_ADMINISTRATIVO == Id).ToList();
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
        /// <param name="Entity">objeto de tipo "ESTATUS_ADMINISTRATIVO" con valores a insertar</param>
        public void Insertar(ESTATUS_ADMINISTRATIVO Entity)
        {
            try
            {
                //Entity.ID_ESTADO_CIVIL = GetSequence<short>("ESTADO_CIVIL_SEQ");
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
        /// <param name="Entity">objeto de tipo "ESTATUS_ADMINISTRATIVO" con valores a actualizar</param>
        public void Actualizar(ESTATUS_ADMINISTRATIVO Entity)
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
                var Entity = GetData().Where(w => w.ID_ESTATUS_ADMINISTRATIVO == Id).SingleOrDefault();
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