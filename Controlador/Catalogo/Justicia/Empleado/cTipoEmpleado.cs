using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTipoEmpleado: EntityManagerServer<TIPO_EMPLEADO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTipoEmpleado()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ALIAS"</returns>
        public IQueryable<TIPO_EMPLEADO> ObtenerTodos()
        {
            try
            {
                return  GetData().OrderBy(w => w.DESCR);
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
        /// <returns>objeto de tipo "ALIAS"</returns>
        public TIPO_EMPLEADO Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_TIPO_EMPLEADO == Id).SingleOrDefault();
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
        public short Insertar(TIPO_EMPLEADO Entity)
        {
            try
            {
                Entity.ID_TIPO_EMPLEADO = GetIDProceso<short>("TIPO_EMPLEADO", "ID_TIPO_EMPLEADO", "1=1");
                if (Insert(Entity))
                    return Entity.ID_TIPO_EMPLEADO;
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
        /// <param name="Entity">objeto de tipo "ALIAS" con valores a actualizar</param>
        public bool Actualizar(TIPO_EMPLEADO Entity)
        {
            try
            {
                return Update(Entity);
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
        public bool Eliminar(TIPO_EMPLEADO Entity)
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