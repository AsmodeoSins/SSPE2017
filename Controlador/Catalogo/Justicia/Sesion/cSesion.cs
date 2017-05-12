using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;
using System.Transactions;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSesion : EntityManagerServer<SESION>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cSesion()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "DECOMISO"</returns>
        public IQueryable<SESION> ObtenerTodos(string usuario = "",string  activo = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<SESION>();
                if (!string.IsNullOrEmpty(usuario))
                    predicate = predicate.And(w =>  w.USUARIO.Trim() == usuario.Trim());
                if (!string.IsNullOrEmpty(activo))
                    predicate = predicate.And(w => w.ACTIVO == activo);
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
        /// <returns>objeto de tipo "DECOMISO"</returns>
        public IQueryable<SESION> Obtener(string sesion)
        {
            try
            {
                return GetData().Where(w => w.ID_SESION == sesion);
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
        public string Insertar(SESION Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    #region Cierra Sesiones Anteriores
                    var sesiones = Context.SESION.Where(w => w.USUARIO.Trim() == Entity.USUARIO.Trim() && w.ACTIVO == "S");
                    if(sesiones != null)
                    {
                        foreach (var s in sesiones)
                        {
                            s.ACTIVO = "N";
                            Context.Entry(s).State = EntityState.Modified;
                        }
                    }
                    #endregion
                    Entity.ID_SESION = GetEjecutaSingleQueries<string>("SELECT RAWTOHEX(SYS_GUID()) FROM DUAL");
                    Context.SESION.Add(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_SESION;
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
        public bool Actualizar(SESION Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.SESION.Attach(Entity);
                    Context.Entry(Entity).Property(x => x.ACTIVO).IsModified = true;
                    Context.Entry(Entity).Property(x => x.FECHA_CONTROL).IsModified = true;
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

      
    }
}