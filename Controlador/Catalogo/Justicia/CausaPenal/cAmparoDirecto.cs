using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;
using System.Data;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAmparoDirecto : EntityManagerServer<AMPARO_DIRECTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAmparoDirecto()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<AMPARO_DIRECTO> ObtenerTodos(short? Centro = null,short? Anio = null,int? Imputado = null,short? Ingreso = null,short? CausaPenal = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<AMPARO_DIRECTO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso != null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (CausaPenal != null)
                    predicate = predicate.And(w => w.ID_CAUSA_PENAL == CausaPenal);
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
        /// <
        /// name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<AMPARO_DIRECTO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_AMPARO_DIRECTO == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a insertar</param>
        public short Insertar(AMPARO_DIRECTO Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    Entity.ID_AMPARO_DIRECTO = GetIDProceso<short>("AMPARO_DIRECTO", "ID_AMPARO_DIRECTO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_CAUSA_PENAL = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_CAUSA_PENAL));
                    var sentencia = Context.SENTENCIA.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL && w.ESTATUS == "A").FirstOrDefault();
                    if (sentencia != null)
                    {
                        Entity.ID_SENTENCIA = sentencia.ID_SENTENCIA;

                        if (Entity.ID_SEN_AMP_RESULTADO == 3)
                        {
                            sentencia.ESTATUS = "I";
                            Context.Entry(sentencia).State = EntityState.Modified;
                        }
                    }

                    Context.AMPARO_DIRECTO.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_AMPARO_DIRECTO;
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
        /// <param name="Entity">objeto de tipo "DECOMISO" con valores a actualizar</param>
        public bool Actualizar(AMPARO_DIRECTO Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var sentencia = Context.SENTENCIA.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL && w.ESTATUS == "A").FirstOrDefault();
                    if (sentencia != null)
                    {
                        if (Entity.ID_SEN_AMP_RESULTADO == 3)
                        {
                            sentencia.ESTATUS = "I";
                            Context.Entry(sentencia).State = EntityState.Modified;
                        }
                    }
                    //if (Update(Entity))
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
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Centro, int Anio, int Imputado, int Ingreso,int CausaPenal, int Id)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso &&  w.ID_CAUSA_PENAL == CausaPenal && w.ID_AMPARO_DIRECTO == Id);
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

        #region Transacciones
        public short InsertarTransaccion(AMPARO_DIRECTO Entity)
        {
            try
            {
                short resultado = 0;
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Entity.ID_AMPARO_DIRECTO = GetIDProceso<short>("AMPARO_DIRECTO", "ID_AMPARO_DIRECTO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_CAUSA_PENAL = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_CAUSA_PENAL));
                    Context.AMPARO_DIRECTO.Add(Entity);
                    resultado = 1;
                    #region Reposicion de Procedimiento
                    if (Entity.ID_SEN_AMP_RESULTADO == 3)//ORDENA REPOSICION DEL PROCEDIMIENTO
                    {
                        var cp = Context.CAUSA_PENAL.FirstOrDefault(w =>
                            w.ID_CENTRO == Entity.ID_CENTRO &&
                            w.ID_ANIO == Entity.ID_ANIO &&
                            w.ID_IMPUTADO == Entity.ID_IMPUTADO &&
                            w.ID_INGRESO == Entity.ID_INGRESO &&
                            w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL);
                        if (cp != null)
                        {
                            cp.ID_ESTATUS_CP = (short)enumCausaPenalEstatus.EN_PROCESO;
                            resultado = 2;
                            if (cp.SENTENCIA != null)
                            {
                                var s = cp.SENTENCIA.FirstOrDefault(w => w.ESTATUS == "A");
                                if (s != null)
                                {
                                    s.ESTATUS = "I";
                                    resultado = 3;
                                }
                            }
                        }
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public short ActualizarTransaccion(AMPARO_DIRECTO Entity)
        {
            try
            {
                short resultado = 0;
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = EntityState.Modified;
                    resultado = 1;
                    #region Reposicion de Procedimiento
                    if (Entity.ID_SEN_AMP_RESULTADO == 3)//ORDENA REPOSICION DEL PROCEDIMIENTO
                    {
                        var cp = Context.CAUSA_PENAL.FirstOrDefault(w =>
                            w.ID_CENTRO == Entity.ID_CENTRO &&
                            w.ID_ANIO == Entity.ID_ANIO &&
                            w.ID_IMPUTADO == Entity.ID_IMPUTADO &&
                            w.ID_INGRESO == Entity.ID_INGRESO &&
                            w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL);
                        if (cp != null)
                        {
                            cp.ID_ESTATUS_CP = (short)enumCausaPenalEstatus.EN_PROCESO;
                            resultado = 2;
                            if (cp.SENTENCIA != null)
                            {
                                var s = cp.SENTENCIA.FirstOrDefault(w => w.ESTATUS == "A");
                                if (s != null)
                                {
                                    s.ESTATUS = "I";
                                    resultado = 3;
                                }
                            }
                        }
                    }
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        #endregion
    }

    public enum enumCausaPenalEstatus
    {
        ACTIVO = 1,
        SUSPENDIDO = 2,
        SOBRESEIDO = 3,
        CONCLUIDO = 4,
        POR_COMPURGAR = 0,
        INACTIVO = 5,
        EN_PROCESO = 6
    }
}