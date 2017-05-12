using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEMIUltimosEmpleos : EntityManagerServer<EMI_ULTIMOS_EMPLEOS>
    {
        #region Constructor
        public cEMIUltimosEmpleos() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<EMI_ULTIMOS_EMPLEOS> Obtener()
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
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto de la tabla</returns>
        public IQueryable<EMI_ULTIMOS_EMPLEOS> Obtener(int id)
        {
            try
            {
                return GetData().Where(w => w.ID_EMI == id);
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
        public string Agregar(EMI_ULTIMOS_EMPLEOS Entidad)
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
                return "Ocurri\u00F3 un error: " + ex.Message;
            }
        }
       
        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidades">Entidades a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public string Agregar(List<EMI_ULTIMOS_EMPLEOS> Entidades)
        {
            try
            {

                if (Insert(Entidades))
                    return "Informaci\u00F3n registrada correctamente.";
                else
                    return "No se registr\u00F3 la informaci\u00F3n.";
            }
            catch (Exception ex)
            {
                return "Ocurri\u00F3 un error: " + ex.Message;
            }
        }

        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidad">Guarda un listado de entidades</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public bool Agregar(int Id, int Consecutivo, List<EMI_ULTIMOS_EMPLEOS> list)
        {
            try
            {
                Eliminar(Id, Consecutivo);
                if (list != null)
                {
                    if (list.Count > 0)
                        return Insert(list);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Actualizar(EMI_ULTIMOS_EMPLEOS Entidad)
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
        public string Eliminar(EMI_ULTIMOS_EMPLEOS Entidad)
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

        /// <summary>
        /// Método que elimina los ultimos empleos
        /// </summary>
        /// <param name="Id">Id de los Emi a eliminar.</param>
        /// <param name="Consecutivo">Consecutivo forma la llave de la tabla.</param>
        /// <returns>Boolean con el resultado de la operación.</returns>
        public bool Eliminar(int Id,int Consecutivo)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_EMI == Id && w.ID_EMI_CONS == Consecutivo);
                foreach (var entity in ListEntity)
                {
                    Delete(entity);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                //return "Ocurri\u00F3 un error: " + ex.Message;
            }
        }
        #endregion
    }
}
