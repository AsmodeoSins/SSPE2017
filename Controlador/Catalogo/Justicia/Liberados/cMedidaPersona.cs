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
    public class cMedidaPersona : EntityManagerServer<MEDIDA_PERSONA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cMedidaPersona()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<MEDIDA_PERSONA> ObtenerTodos(short? Centro = null, short? Anio = null, int? Imputado = null, short? ProcesoLibertad = null, short? MedidaLiberado = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<MEDIDA_PERSONA>();
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
        /// <returns>objeto de tipo "FUERO"</returns>
        public MEDIDA_PERSONA Obtener(short? Centro = null, short? Anio = null, int? Imputado = null, short? ProcesoLibertad = null, short? MedidaLiberado = null,short? Persona = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<MEDIDA_PERSONA>();
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
                if (Persona.HasValue)
                    predicate = predicate.And(w => w.ID_PERSONA == MedidaLiberado);
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
        /// <param name="Entity">objeto de tipo "FUERO" con valores a insertar</param>
        public short Insertar(MEDIDA_PERSONA Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_PERSONA = GetIDProceso<short>("MEDIDA_PERSONA", "ID_PERSONA", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_PROCESO_LIBERTAD = {3} AND ID_MEDIDA_LIBERADO = {4}",
                        Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_PROCESO_LIBERTAD, Entity.ID_MEDIDA_LIBERADO));
                    Context.MEDIDA_PERSONA.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_PERSONA;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "FUERO" con valores a actualizar</param>
        public bool Actualizar(MEDIDA_PERSONA Entity)
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
        public bool Eliminar(MEDIDA_PERSONA Entity)
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