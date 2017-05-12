using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEmisor : EntityManagerServer<EMISOR>
    {
        #region Constructor
        public cEmisor() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene los centros de la tabla emisor (plataforma mexico)
        /// </summary>
        /// <param name="PM">El tipo de emisor que se va a consultar, 1=De plataforma mexico, 0=No de plataforma mexico</param>
        /// <returns></returns>
        public IQueryable<EMISOR> ObtenerporTipo(int? PM=null)
        {
            try
            {
                if (PM.HasValue)
                    return GetData(w => w.PM == PM.Value);
                else
                    return GetData();
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<EMISOR> Obtener()
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
        public EMISOR Obtener(int id)
        {
            try
            {
                return GetData(w=>w.ID_EMISOR == id).SingleOrDefault();
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
        public string Agregar(EMISOR Entidad)
        {
            try
            {

                if (Insert(Entidad))
                    return "Informaci\u00F3n registrada correctamente.";
                else
                    return "No se registr\u00F3 la informaci\u00F3n.";
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    return "Ocurri\u00F3 un error: " + ex.InnerException.Message;
                }
                else
                    return "Ocurri\u00F3 un error: " + ex.Message;
            }
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Actualizar(EMISOR Entidad)
        {
            try
            {
                if (Update(Entidad))
                    return "Informaci\u00F3n actualizada correctamente.";
                else
                    return "No se pudo actualizar la informaci\u00F3n, intente de nuevo.";
            }
            catch (Exception ex)
            {
                return "Ocurri\u00F3 un error: " + ex.Message;
            }
        }
        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Eliminar(EMISOR Entidad)
        {
            try
            {
                if (Delete(Entidad))
                    return "Informaci\u00F3n eliminada correctamente";
                else
                    return "No se pudo eliminar la informaci\u00F3n, intente de nuevo.";
            }
            catch (Exception ex)
            {
                return "Ocurri\u00F3 un error: " + ex.Message;
            }
        }
        #endregion
    }
}
