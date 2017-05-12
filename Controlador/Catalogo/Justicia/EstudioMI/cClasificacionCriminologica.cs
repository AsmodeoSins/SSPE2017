using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using System.Collections.Generic;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cClasificacionCriminologica : EntityManagerServer<CLASIFICACION_CRIMINOLOGICA>
    {
        #region Constructor
        public cClasificacionCriminologica() { }
        #endregion


        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<CLASIFICACION_CRIMINOLOGICA> Obtener()
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
        public CLASIFICACION_CRIMINOLOGICA Obtener(int id)
        {
            try
            {
                return GetData(w => w.ID_CLAS == id).SingleOrDefault();
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
        public string Agregar(CLASIFICACION_CRIMINOLOGICA Entidad)
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
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "EMI_GRUPO_FAMILIAR" con valores a insertar</param>
        //public bool Insertar(int Id, int Consecutivo, List<CLASIFICACION_CRIMINOLOGICA> list)
        //{
        //    try
        //    {
        //        Eliminar(Id, Consecutivo);
        //        if (list != null)
        //            if (list.Count > 0)
        //                return Insert(list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Actualizar(CLASIFICACION_CRIMINOLOGICA Entidad)
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
        public string Eliminar(CLASIFICACION_CRIMINOLOGICA Entidad)
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
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        //public bool Eliminar(int Id, int Consecutivo)
        //{
        //    try
        //    {
        //        var ListEntity = GetData().Where(w => w..ID_EMI == Id && w.ID_EMI_CONS == Consecutivo);
        //        foreach (var entity in ListEntity)
        //        {
        //            Delete(entity);
        //        }
        //        return true;

        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.InnerException != null)
        //        {
        //            if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
        //                throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
        //        }
        //        throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
        //    }
        //    return false;
        //}
        #endregion
    }
}
