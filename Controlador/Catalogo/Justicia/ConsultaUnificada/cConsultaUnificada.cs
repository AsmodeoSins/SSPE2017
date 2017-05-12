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
    public class cConsultaUnificada : EntityManagerServer<CONSULTA_UNIFICADA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cConsultaUnificada()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "cConsultaUnificada"</returns>
        public IQueryable<CONSULTA_UNIFICADA> ObtenerTodos(string Nombre,short? Clasificacion = null,int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<CONSULTA_UNIFICADA>();
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.NOMBRE.Contains(Nombre));
                if(Clasificacion != null)
                    if(Clasificacion != -1)
                        predicate = predicate.And(w => w.ID_CLASIFICACION == Clasificacion);
                predicate = predicate.And(w => w.ESTATUS == "S");
                return GetData(predicate.Expand()).OrderByDescending(w => w.NOMBRE).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<CONSULTA_UNIFICADA> ObtenerTodos(string Nombre, string Estatus)
        {
            try
            {
                var predicate = PredicateBuilder.True<CONSULTA_UNIFICADA>();
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.NOMBRE.Contains(Nombre));
                if (!string.IsNullOrEmpty(Estatus))
                    predicate = predicate.And(w => w.ESTATUS == Estatus);
                return GetData(predicate.Expand()).OrderByDescending(w => w.NOMBRE);
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
        /// <returns>objeto de tipo "CONSULTA_UNIFICADA"</returns>
        public IQueryable<CONSULTA_UNIFICADA> Obtener(short Id)
        {
            try
            {
                return GetData().Where(w => w.ID_CONSULTA == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "CONSULTA_UNIFICADA" con valores a insertar</param>
        public short Insertar(CONSULTA_UNIFICADA Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_CONSULTA = GetIDProceso<short>("CONSULTA_UNIFICADA", "ID_CONSULTA", string.Format("ID_CLASIFICACION = {0}", Entity.ID_CLASIFICACION));
                    Context.CONSULTA_UNIFICADA.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_CONSULTA;
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
        /// <param name="Entity">objeto de tipo "CONSULTA_UNIFICADA" con valores a actualizar</param>
        public bool Actualizar(CONSULTA_UNIFICADA Entity)
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
        public bool Eliminar(CONSULTA_UNIFICADA Entity)
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