using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEMIFamConDelito : EntityManagerServer<EMI_ANTECEDENTE_FAM_CON_DEL>
    {
        #region Constructor
        public cEMIFamConDelito() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<EMI_ANTECEDENTE_FAM_CON_DEL> Obtener()
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

        public IQueryable<EMI_ANTECEDENTE_FAM_CON_DEL> Obtener(int Id,short Cons)
        {
            try
            {
                return GetData().Where(w => w.ID_EMI == Id && w.ID_EMI_CONS == Cons);
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
        public string Agregar(EMI_ANTECEDENTE_FAM_CON_DEL Entidad)
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
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "EMI_GRUPO_FAMILIAR" con valores a insertar</param>
        public bool Insertar(int Id, int Consecutivo, List<EMI_ANTECEDENTE_FAM_CON_DEL> Delito,List<EMI_ANTECEDENTE_FAMILIAR_DROGA> Droga)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    #region Grupo Familiar Con Delito
                    var delitos = Context.EMI_ANTECEDENTE_FAM_CON_DEL.Where(w => w.ID_EMI == Id && w.ID_EMI_CONS == Consecutivo);
                    if (delitos != null)
                    {
                        foreach (var d in delitos)
                        {
                            Context.Entry(d).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var d in Delito)
                    {
                        Context.EMI_ANTECEDENTE_FAM_CON_DEL.Add(d);
                    }
                    #endregion
                    #region Grupo Familiar Con Droga
                    var drogas = Context.EMI_ANTECEDENTE_FAMILIAR_DROGA.Where(w => w.ID_EMI == Id && w.ID_EMI_CONS == Consecutivo);
                    if (drogas != null)
                    {
                        foreach (var d in drogas)
                        {
                            Context.Entry(d).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var d in Droga)
                    {
                        Context.EMI_ANTECEDENTE_FAMILIAR_DROGA.Add(d);
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //Eliminar(Id, Consecutivo);
                //if (list != null)
                //    if (list.Count > 0)
                //        return Insert(list);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
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
        public string Actualizar(EMI_ANTECEDENTE_FAM_CON_DEL Entidad)
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
        public string Eliminar(EMI_ANTECEDENTE_FAM_CON_DEL Entidad)
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
        public bool Eliminar(int Id, int Consecutivo)
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
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            return false;
        }
        #endregion
    }
}
