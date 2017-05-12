using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTipoAutoridadSalida : EntityManagerServer<TIPO_AUTORIDAD_SALIDA>
    {
        #region Constructor
        public cTipoAutoridadSalida() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<TIPO_AUTORIDAD_SALIDA> ObtenerTodo()
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
        public TIPO_AUTORIDAD_SALIDA Obtener(int id)
        {
            try
            {
                return GetData(w=>w.ID_AUTORIDAD_SALIDA == id).SingleOrDefault();
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
        public bool Agregar(TIPO_AUTORIDAD_SALIDA Entidad)
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
        public bool Actualizar(TIPO_AUTORIDAD_SALIDA Entidad)
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
        public bool Eliminar(TIPO_AUTORIDAD_SALIDA Entidad)
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
