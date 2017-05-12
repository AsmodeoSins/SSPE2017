using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using System.Transactions;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEMISituacionJuridica : EntityManagerServer<EMI_SITUACION_JURIDICA>
    {
        #region Constructor
        public cEMISituacionJuridica() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<EMI_SITUACION_JURIDICA> Obtener()
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
        /// <returns>Objeto de la tabla.</returns>
        public EMI_SITUACION_JURIDICA Obtener(int id)
        {
            try
            {
                return GetData(w=>w.ID_EMI==id).SingleOrDefault();
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
        public bool Agregar(EMI_SITUACION_JURIDICA Entidad)
        {
            try
            {
                if (Insert(Entidad))
                    return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
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
        public bool Actualizar(EMI_SITUACION_JURIDICA Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    Context.Entry(Entidad).State = EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                    //if (Insert(Entity))
                }
               // if (Update(Entidad))
               //     return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
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
        public string Eliminar(EMI_SITUACION_JURIDICA Entidad)
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
