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
    public class cMedidaPresentacionDetalle : EntityManagerServer<MEDIDA_PRESENTACION_DETALLE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cMedidaPresentacionDetalle()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<MEDIDA_PRESENTACION_DETALLE> ObtenerTodos()
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
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "FUERO"</returns>
        public MEDIDA_PRESENTACION_DETALLE Obtener(short? Centro = null, short? Anio = null, int? Imputado = null, short? ProcesoLibertad = null, short? MedidaLiberado = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<MEDIDA_PRESENTACION_DETALLE>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (ProcesoLibertad.HasValue)
                    predicate = predicate.And(w => w.ID_PROCESO_LIBERTAD == ProcesoLibertad);
                if (MedidaLiberado.HasValue)
                    predicate = predicate.And(w => w.ID_MEDIDA_LIBERADO == MedidaLiberado);
                return GetData(predicate.Expand()).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "FUERO" con valores a insertar</param>
        public bool Insertar(MEDIDA_PRESENTACION_DETALLE Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.MEDIDA_PRESENTACION_DETALLE.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Actualizacion de la tabla MEDIDA_PRESENTACION_DETALLE
        /// </summary>
        /// <param name="Entity"></param>
        /// <returns></returns>
        public bool Actualizar(MEDIDA_PRESENTACION_DETALLE Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
      
        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(MEDIDA_PRESENTACION_DETALLE Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var obj = Context.MEDIDA_PRESENTACION_DETALLE.Where(w =>
                        w.ID_CENTRO == Entity.ID_CENTRO &&
                        w.ID_ANIO == Entity.ID_ANIO &&
                        w.ID_IMPUTADO == Entity.ID_IMPUTADO &&
                        w.ID_PROCESO_LIBERTAD == Entity.ID_PROCESO_LIBERTAD &&
                        w.ID_MEDIDA_LIBERADO == Entity.ID_MEDIDA_LIBERADO &&
                        w.ID_DETALLE == Entity.ID_DETALLE).FirstOrDefault();

                    if (obj != null)
                    {
                        Context.MEDIDA_PRESENTACION_DETALLE.Remove(obj);
                        Context.SaveChanges();
                        transaccion.Complete();
                        return true;
                    }
                    else
                        return true;
                }
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
        
    }
}