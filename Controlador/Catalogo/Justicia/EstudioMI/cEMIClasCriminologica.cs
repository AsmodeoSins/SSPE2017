using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEMIClasCriminologica : EntityManagerServer<EMI_CLAS_CRIMINOLOGICA>
    {
        #region Constructor
        public cEMIClasCriminologica() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<EMI_CLAS_CRIMINOLOGICA> Obtener()
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
        /// Obtiene toda la informacion de un registro de la tabla.
        /// </summary>
        /// <returns>Objeto de la tabla</returns>
        public EMI_CLAS_CRIMINOLOGICA Obtener(int idEmi, short idEmiCons)
        {
            try
            {
                return GetData(w=>w.ID_EMI == idEmi && w.ID_EMI_CONS == idEmiCons).SingleOrDefault();
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
        public bool Agregar(EMI_CLAS_CRIMINOLOGICA Entidad,List<EMI_SANCION_DISCIPLINARIA> Sancion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.EMI_CLAS_CRIMINOLOGICA.Add(Entidad);
                    #region Sanciones Disiplinarias
                    var sanciones = Context.EMI_SANCION_DISCIPLINARIA.Where(w => w.ID_EMI == Entidad.ID_EMI && w.ID_EMI_CONS == Entidad.ID_EMI_CONS);
                    if (sanciones != null)
                    {
                        foreach (var s in sanciones)
                        {
                            Context.Entry(s).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var s in Sancion)
                    {
                        Context.EMI_SANCION_DISCIPLINARIA.Add(s);
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Insert(Entidad))
                //    return true;
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
        public bool Actualizar(EMI_CLAS_CRIMINOLOGICA Entidad, List<EMI_SANCION_DISCIPLINARIA> Sancion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entidad).State = System.Data.EntityState.Modified;
                    #region Sanciones Disiplinarias
                    var sanciones = Context.EMI_SANCION_DISCIPLINARIA.Where(w => w.ID_EMI == Entidad.ID_EMI && w.ID_EMI_CONS == Entidad.ID_EMI_CONS);
                    if (sanciones != null)
                    {
                        foreach (var s in sanciones)
                        {
                            Context.Entry(s).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var s in Sancion)
                    {
                        Context.EMI_SANCION_DISCIPLINARIA.Add(s);
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
        public string Eliminar(EMI_CLAS_CRIMINOLOGICA Entidad)
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
