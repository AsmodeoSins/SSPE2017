using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEMIEstatusPrograma : EntityManagerServer<EMI_ESTATUS_PROGRAMA>
    {
        #region Constructor
        public cEMIEstatusPrograma() { }
        #endregion
        #region Obtener
        public ObservableCollection<EMI_ESTATUS_PROGRAMA> ObtenerTodos()
        {
            return new ObservableCollection<EMI_ESTATUS_PROGRAMA>(GetData());
        }
        public EMI_ESTATUS_PROGRAMA Obtener(int id)
        {
            return GetData(w => w.ID_ESTATUS == id).FirstOrDefault();
        }
        #endregion
        #region Agregar
        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidad">Entidad a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public string Agregar(EMI_ESTATUS_PROGRAMA Entidad)
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
        /// <param name="Entidades">Lista de entidades a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public string Agregar(List<EMI_ESTATUS_PROGRAMA> Entidades)
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
        #endregion
        #region Actualizar
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Actualizar(EMI_ESTATUS_PROGRAMA Entidad)
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
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidades">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Actualizar(List<EMI_ESTATUS_PROGRAMA> Entidades)
        {
            try
            {
                if (Update(Entidades))
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
    }
}
