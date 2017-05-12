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
    public class cProcesoLibertadSeguimiento : EntityManagerServer<PROCESO_LIBERTAD_SEGUIMIENTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cProcesoLibertadSeguimiento()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "PROCESO_LIBERTAD_SEGUIMIENTO"</returns>
        public IQueryable<PROCESO_LIBERTAD_SEGUIMIENTO> ObtenerTodos(short? Centro = null, short? Anio = null, int? Imputado = null,int? Proceso = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO_LIBERTAD_SEGUIMIENTO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if(Proceso.HasValue)
                    predicate = predicate.And(w => w.ID_PROCESO_LIBERTAD == Proceso);
                return GetData(predicate.Expand());
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
        /// <returns>objeto de tipo "PROCESO_LIBERTAD_SEGUIMIENTO"</returns>
        public PROCESO_LIBERTAD_SEGUIMIENTO Obtener(short? Centro = null, short? Anio = null, int? Imputado = null,int? Proceso = null, short? Id = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO_LIBERTAD_SEGUIMIENTO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Proceso.HasValue)
                    predicate = predicate.And(w => w.ID_PROCESO_LIBERTAD == Proceso);
                if (Id.HasValue)
                    predicate = predicate.And(w => w.ID_CONSEC == Id);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "FUPROCESO_LIBERTAD_SEGUIMIENTOERO" con valores a insertar</param>
        public decimal Insertar(PROCESO_LIBERTAD_SEGUIMIENTO Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_CONSEC = GetIDProceso<short>("PROCESO_LIBERTAD_SEGUIMIENTO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_PROCESO_LIBERTAD = {3}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_PROCESO_LIBERTAD));
                    Context.PROCESO_LIBERTAD_SEGUIMIENTO.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_CONSEC;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(PROCESO_LIBERTAD_SEGUIMIENTO Entity)
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
        public bool Eliminar(PROCESO_LIBERTAD_SEGUIMIENTO Entity)
        {
            try
            {
                    return Delete(Entity);
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