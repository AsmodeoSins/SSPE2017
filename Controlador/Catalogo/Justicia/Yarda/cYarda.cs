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
    public class cYarda : EntityManagerServer<YARDA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cYarda()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "YARDA"</returns>
        public IQueryable<YARDA> ObtenerTodos(short? Centro = null, short? Edificio = null, short? Sector = null, short? DiaSemana = null, int? Minuto =  null)
        {
            try
            {
                var predicate = PredicateBuilder.True<YARDA>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Edificio != null)
                    predicate = predicate.And(w => w.ID_EDIFICIO == Edificio);
                if (Sector != null)
                    predicate = predicate.And(w => w.ID_SECTOR == Sector);
                if (DiaSemana != null)
                    predicate = predicate.And(w => w.SEMANA_DIA == DiaSemana);
                if (Minuto != null)
                    predicate = predicate.And(w => (w.HORA_FIN * 60 + w.MINUTO_FIN) > Minuto);
                predicate = predicate.And(w => w.ESTATUS == "S");
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
        /// <returns>objeto de tipo "YARDA"</returns>
        public IQueryable<YARDA> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_YARDA == Id);
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
        public int Insertar(YARDA Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_YARDA = GetIDProceso<int>("YARDA", "ID_YARDA", "1=1");
                    Context.YARDA.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_YARDA;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "YARDA" con valores a actualizar</param>
        public bool Actualizar(YARDA Entity)
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

        public bool ActualizarEstatus(YARDA Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.YARDA.Attach(Entity);
                    Context.Entry(Entity).Property(x => x.ESTATUS).IsModified = true;
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
        public bool Eliminar(YARDA Entity)
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