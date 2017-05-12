using System.Linq;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadMotivo : SSP.Modelo.EntityManagerServer<SSP.Servidor.PERSONALIDAD_MOTIVO>
    {
        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<SSP.Servidor.PERSONALIDAD_MOTIVO> ObtenerTodos()
        {
            try
            {
                return GetData();
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<SSP.Servidor.PERSONALIDAD_MOTIVO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_MOTIVO == Id);
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a insertar</param>
        public bool Insertar(SSP.Servidor.PERSONALIDAD_MOTIVO Entity)
        {
            try
            {

                Entity.ID_MOTIVO = GetSequence<short>("PERSONALIDAD_MOTIVO_SEQ");
                if (Insert(Entity))
                    return true;
                else
                    return false;

            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public void Actualizar(SSP.Servidor.PERSONALIDAD_MOTIVO Entity)
        {
            try
            {
                Update(Entity);
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new System.ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new System.ApplicationException(ex.Message);
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
                var ListEntity = GetData().Where(w => w.ID_MOTIVO == Id);
                foreach (var entity in ListEntity)
                {
                    Delete(entity);
                }
                return true;

            }
            catch (System.Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new System.ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new System.ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            return false;
        }

    }
}
