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
    public class cAtencionServicio : EntityManagerServer<ATENCION_SERVICIO>
    {
        #region Constructor
        public cAtencionServicio() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<ATENCION_SERVICIO> ObtenerTodo()
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

        public ATENCION_SERVICIO ObtenerXID(short atencion, short servicio)
        {
            try
            {
                return GetData(w => w.ID_TIPO_ATENCION == atencion && w.ID_TIPO_SERVICIO == servicio).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<ATENCION_SERVICIO> ObtenerXAtencion(short atencion)
        {
            try
            {
                return GetData(w => w.ID_TIPO_ATENCION == atencion);
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
        public int Agregar(ATENCION_SERVICIO Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var ent = GetData().OrderByDescending(o => o.ID_TIPO_SERVICIO).First(f => f.ID_TIPO_ATENCION == Entidad.ID_TIPO_ATENCION);
                    Entidad.ID_TIPO_SERVICIO = (short)(ent != null ? ent.ID_TIPO_SERVICIO + 1 : 1);
                    Context.ATENCION_SERVICIO.Add(Entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entidad.ID_TIPO_ATENCION;
                }
                //if (Insert(Entidad))
                //    return Entidad.ID_ATENCION;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Método de inserción
        /// </summary>
        /// <param name="Entidad">Entidad a guardar en la tabla</param>
        /// <returns>Cadena de texto con el resultado correspondiente.</returns>
        public bool Actualizar(ATENCION_SERVICIO Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entidad).State = EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Insert(Entidad))
                //    return Entidad.ID_ATENCION;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region Eliminación
        /// <summary>
        /// Método que elimina una entidad de la BD.
        /// </summary>
        /// <param name="Entidad">Entidad a eliminar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Eliminar(ATENCION_SERVICIO Entidad)
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
