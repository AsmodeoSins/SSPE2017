using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cIncidenteTipo : EntityManagerServer<INCIDENTE_TIPO>
    {
        #region Constructor
        public cIncidenteTipo() { }
        #endregion
        #region Obtener
        public IQueryable<INCIDENTE_TIPO> ObtenerTodas()
        {
           return GetData();
        }
        #endregion
        #region Insertar
        /// <summary>
        /// Inserta en la tabla la entidad enviada.
        /// </summary>
        /// <param name="entidad">Objeto que se desea insertar en la tabla</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public int Agregar(INCIDENTE_TIPO entidad)
        {
            try
            {
                entidad.ID_INCIDENTE_TIPO = GetIDProceso<short>("INCIDENTE_TIPO", "ID_INCIDENTE_TIPO", "1=1");
                if (Insert(entidad))
                    return entidad.ID_INCIDENTE_TIPO;
                else
                    return 0;
            }
            catch (Exception ex)
            { 
                return 0;
            }
        }
      
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(INCIDENTE_TIPO Entidad)
        {
            try
            {
                if (Update(Entidad))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    
        #endregion
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Eliminar(INCIDENTE_TIPO Entidad)
        {
            try
            {
                if (Delete(Entidad))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        #endregion
    }
}
