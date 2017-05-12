using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia.EstudioMI
{
    public class cEMIUsoDroga : EntityManagerServer<EMI_USO_DROGA>
    {
        #region Constructor
        public cEMIUsoDroga() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<EMI_USO_DROGA> Obtener(int idEmi)
        {
            try
            {
                return GetData(w=>w.ID_EMI == idEmi);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<EMI_USO_DROGA> Obtener(int Id,short Cons)
        {
            try
            {
                return GetData(w => w.ID_EMI == Id && w.ID_EMI_CONS == Cons);
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
        public EMI_USO_DROGA Obtener(int id, int id_droga)
        {
            try
            {
                return GetData(w=>w.ID_EMI == id&& w.ID_DROGA == id_droga).SingleOrDefault();
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
        public string Agregar(EMI_USO_DROGA Entidad)
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

        public bool Insertar(int Id, int Consecutivo, List<EMI_USO_DROGA> list)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var drogas = Context.EMI_USO_DROGA.Where(w => w.ID_EMI == Id && w.ID_EMI_CONS == Consecutivo);
                    if (drogas != null)
                    {
                        foreach (var d in drogas)
                        {
                            Context.Entry(d).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var d in list) 
                    {
                        Context.EMI_USO_DROGA.Add(d);
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                    //if (Insert(Entity))
                }
                //Eliminar(Id, Consecutivo);
                //if (list != null)
                //    if (list.Count > 0)
                //        return Insert(list);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidades">Lista de entidades a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public string Agregar(List<EMI_USO_DROGA> Entidades)
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
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Actualizar(EMI_USO_DROGA Entidad)
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
        public string Actualizar(List<EMI_USO_DROGA> Entidades)
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
        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public string Eliminar(EMI_USO_DROGA Entidad)
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
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        //public string Eliminar(int idEmi, int idCons)
        //{
        //    try
        //    {
        //        if (Delete(GetData(w=>w.ID_EMI == idEmi && w.ID_EMI_CONS == idCons).ToList()))
        //            return "Informaci\u00F3n eliminada correctamente";
        //        else
        //            return "No se pudo eliminar la informaci\u00F3n, intente de nuevo.";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Ocurri\u00F3 un error: " + ex.Message;
        //    }
        //}
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
