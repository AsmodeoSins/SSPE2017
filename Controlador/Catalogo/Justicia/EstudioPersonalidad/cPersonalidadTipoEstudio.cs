using LinqKit;
using SSP.Servidor;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cPersonalidadTipoEstudio : SSP.Modelo.EntityManagerServer<PERSONALIDAD_TIPO_ESTUDIO>
    {
        public cPersonalidadTipoEstudio() { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "TIPO_DELITO"</returns>
        public IQueryable<PERSONALIDAD_TIPO_ESTUDIO> ObtenerTodos(string buscar = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONALIDAD_TIPO_ESTUDIO>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar));
                return GetData(predicate.Expand()).OrderBy(w => w.DESCR);
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
        /// <returns>objeto de tipo "TIPO_DELITO"</returns>
        public System.Collections.Generic.List<PERSONALIDAD_TIPO_ESTUDIO> Obtener(int TipoDelito)
        {
            var Resultado = new System.Collections.Generic.List<PERSONALIDAD_TIPO_ESTUDIO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_TIPO == TipoDelito).ToList();
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "TIPO_DELITO" con valores a insertar</param>
        public long Insertar(PERSONALIDAD_TIPO_ESTUDIO Entity)
        {
            try
            {
                Entity.ID_TIPO = GetSequence<short>("PERSONALIDAD_TIPO_ESTUDIO_SEQ");
                Insert(Entity);
                return Entity.ID_TIPO;
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "TIPO_DELITO" con valores a actualizar</param>
        public bool Actualizar(PERSONALIDAD_TIPO_ESTUDIO Entity)
        {
            try
            {
                Update(Entity);
                return true;
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new System.ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new System.ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int TipoEstudio)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_TIPO == TipoEstudio).ToList();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
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
        }
    }
}
