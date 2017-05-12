using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using System.Transactions;
using LinqKit;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAtencionRecibida : EntityManagerServer<ATENCION_RECIBIDA>
    {
        #region Constructor
        public cAtencionRecibida() { }
        #endregion
        #region Obtener
        /// <summary>
        /// Obtiene toda la informacion de la tabla.
        /// </summary>
        /// <returns>Objeto IQueryable</returns>
        public IQueryable<ATENCION_RECIBIDA> ObtenerTodo()
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

        public IQueryable<ATENCION_RECIBIDA> ObtenerTodoHistorico(short Centro,short Anio,int Imputado,short? AreaTecnica = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_RECIBIDA>();
                predicate = predicate.And(w => w.ATENCION_CITA.ID_CENTRO == Centro && w.ATENCION_CITA.ID_ANIO == Anio && w.ATENCION_CITA.ID_IMPUTADO == Imputado);
                if (AreaTecnica.HasValue)
                    predicate = predicate.And(w => w.ATENCION_CITA.ATENCION_SOLICITUD.ID_TECNICA == AreaTecnica);
                return GetData(predicate.Expand()).OrderByDescending(w => w.ATENCION_FEC);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public ATENCION_RECIBIDA Obtener(int id)
        {
            try
            {
                return GetData(w => w.ID_CITA == id).SingleOrDefault();
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
        public bool Agregar(ATENCION_RECIBIDA Entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions(){IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted}))
                {
                    if (!Context.ATENCION_RECIBIDA.Any(w => w.ID_CITA == Entidad.ID_CITA))
                    {
                        Context.ATENCION_RECIBIDA.Add(Entidad);
                        //cambiamos el estatus en la taba de atencion cita
                        var obj = new ATENCION_CITA();
                        obj.ID_CITA = Entidad.ID_CITA;
                        obj.ID_CENTRO_UBI = Entidad.ID_CENTRO_UBI;
                        obj.ESTATUS = "S";
                        Context.ATENCION_CITA.Attach(obj);
                        Context.Entry(obj).Property(x => x.ESTATUS).IsModified = true;
                        Context.SaveChanges();
                        transaccion.Complete();
                        return true;
                    }
                    return false;
                }
                //return Insert(Entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            
        }
        #endregion
        #region Actualización
        /// <summary>
        /// Método que actualiza una entidad.
        /// </summary>
        /// <param name="Entidad">Entidad a actualziar.</param>
        /// <returns>Cadena de texto con el resultado de la operación.</returns>
        public bool Actualizar(ATENCION_RECIBIDA Entidad)
        {
            try
            {
                return Update(Entidad);
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
        public bool Eliminar(ATENCION_RECIBIDA Entidad)
        {
            try
            {
               return Delete(Entidad);
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
        }

        #endregion
    }
}
