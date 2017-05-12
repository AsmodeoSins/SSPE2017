using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAreaTecnica : EntityManagerServer<AREA_TECNICA>
    {
        #region Constructor
        public cAreaTecnica() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene el listado de areas tecnicas
        /// </summary>
        /// <param name="buscar">Nombre del area tecnica a buscar</param>
        /// <param name="estatus">Estatus "S" o "N"</param>
        /// <returns></returns>
        public IQueryable<AREA_TECNICA> ObtenerTodo(string buscar = "", string estatus="")
        {
            try
            {
                var predicate = PredicateBuilder.True<AREA_TECNICA>();
                if (!string.IsNullOrWhiteSpace(buscar))
                    predicate = predicate.And(w=>w.DESCR.Contains(buscar));
                if (!string.IsNullOrWhiteSpace(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
                return GetData(predicate.Expand()).OrderBy(w => w.DESCR);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public AREA_TECNICA Obtener(int id)
        {
            try
            {
                return GetData(w => w.ID_TECNICA == id).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
        #region Insercion
        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidad">Entidad a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public short Agregar(AREA_TECNICA Entidad)
        {
            try
            {
                Entidad.ID_TECNICA = GetIDProceso<short>("AREA_TECNICA", "ID_TECNICA", "1=1");
                if (Insert(Entidad))
                    return Entidad.ID_TECNICA;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(AREA_TECNICA Entidad)
        {
            try
            {
                return Update(Entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_TECNICA == Id).SingleOrDefault();
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

        #endregion
    }
}
