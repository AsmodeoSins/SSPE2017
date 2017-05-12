using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using LinqKit;
using System.Transactions;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAtencionSolicitud : EntityManagerServer<ATENCION_SOLICITUD>
    {
        #region Constructor
        public cAtencionSolicitud() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<ATENCION_SOLICITUD> ObtenerTodo()
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

        public ATENCION_SOLICITUD Obtener(int id)
        {
            try
            {
                return GetData(w=>w.ID_ATENCION == id).SingleOrDefault();
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
        public int Agregar(ATENCION_SOLICITUD Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (Entidad.ATENCION_CITA != null && Entidad.ATENCION_CITA.Count>0)
                    {
                        var ac = Entidad.ATENCION_CITA.FirstOrDefault();
                        ac.ID_CITA = GetSequence<int>("ATENCION_CITA_SEQ");
                    }
                    Entidad.ID_ATENCION = GetIDProceso<int>("ATENCION_SOLICITUD", "ID_ATENCION", string.Format("ID_CENTRO={0}",Entidad.ID_CENTRO));
                    Context.ATENCION_SOLICITUD.Add(Entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entidad.ID_ATENCION;
                }
                //if (Insert(Entidad))
                //    return Entidad.ID_ATENCION;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(ATENCION_SOLICITUD Entidad,List<ATENCION_INGRESO> Ingreso)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entidad).State = EntityState.Modified;
                   #region Ingresos
                    var ingresos = Context.ATENCION_INGRESO.Where(w => w.ID_ATENCION == Entidad.ID_ATENCION);
                    if (ingresos != null)
                    {
                        foreach (var i in ingresos)
                        {
                            Context.ATENCION_INGRESO.Remove(i);
                        }
                    }
                    foreach(var i in Ingreso)
                    {
                        Context.ATENCION_INGRESO.Add(i);
                    }
                   #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                    //if (Insert(Entity))
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
        public bool Eliminar(ATENCION_SOLICITUD Entidad)
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
