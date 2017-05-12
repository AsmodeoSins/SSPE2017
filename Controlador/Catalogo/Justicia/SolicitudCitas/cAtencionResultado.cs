using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAtencionResultado : EntityManagerServer<ATENCION_RESULTADO>
    {
        #region Constructor
        public cAtencionResultado() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<ATENCION_RESULTADO> ObtenerTodo()
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

        public ATENCION_RESULTADO Obtener(int id)
        {
            try
            {
                return GetData(w=>w.ID_CITA == id).SingleOrDefault();
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
        public bool Agregar(ATENCION_RESULTADO Entidad)
        {
            try
            {

                if (Insert(Entidad))
                    return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(ATENCION_RESULTADO Entidad)
        {
            try
            {
                if (Update(Entidad))
                    return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Eliminar(ATENCION_RESULTADO Entidad)
        {
            try
            {
                if (Delete(Entidad))
                    return true;
            }
            catch (Exception ex)
            {
                
            }
            return false;
        }
        #endregion
    }
}
