using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEMIFichaIdentificacion:EntityManagerServer<EMI_FICHA_IDENTIFICACION>
    {
        #region Constructor
        public cEMIFichaIdentificacion() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<EMI_FICHA_IDENTIFICACION> Obtener()
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
        /// <returns>Objeto IQueryable</returns>
        public EMI_FICHA_IDENTIFICACION Obtener(int id)
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
        public bool Agregar(EMI_FICHA_IDENTIFICACION Entidad,List<EMI_ULTIMOS_EMPLEOS> Empleo)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.EMI_FICHA_IDENTIFICACION.Add(Entidad);
                    foreach (var e in Empleo)
                    {
                        Context.EMI_ULTIMOS_EMPLEOS.Add(e);
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Insert(Entidad))
                //    return true;
                //else
                //    return false;
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
        public bool Actualizar(EMI_FICHA_IDENTIFICACION Entidad, List<EMI_ULTIMOS_EMPLEOS> Empleo)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    Context.Entry(Entidad).State = System.Data.EntityState.Modified;
                    #region Ultimos Empleos
                    var empleos = Context.EMI_ULTIMOS_EMPLEOS.Where(w => w.ID_EMI == Entidad.ID_EMI && w.ID_EMI_CONS == Entidad.ID_EMI_CONS);
                    if (empleos != null)
                    {
                        foreach (var e in empleos)
                        {
                            Context.Entry(e).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var e in Empleo)
                    {
                        Context.EMI_ULTIMOS_EMPLEOS.Add(e);
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Update(Entidad))
                //    return true;
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
        public string Eliminar(EMI_FICHA_IDENTIFICACION Entidad)
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
