using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cDecomisoFabricante : EntityManagerServer<DECOMISO_FABRICANTE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cDecomisoFabricante()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO_FABRICANTE> ObtenerTodos()
        {
            try
            {
                return GetData();
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
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<DECOMISO_FABRICANTE> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_FABRICANTE == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a insertar</param>
        public bool Insertar(DECOMISO_FABRICANTE Entity)
        {
            try
            {
                Entity.ID_FABRICANTE = GetIDProceso<short>("DECOMISO_FABRICANTE", "ID_FABRICANTE", "1=1"); //GetSequence<short>("DECOMISO_FABRICANTE_SEQ");
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public void Actualizar(DECOMISO_FABRICANTE Entity)
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
        public bool Eliminar(DECOMISO_FABRICANTE Entity)
        {
            try
            {
                if (Delete(Entity))
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