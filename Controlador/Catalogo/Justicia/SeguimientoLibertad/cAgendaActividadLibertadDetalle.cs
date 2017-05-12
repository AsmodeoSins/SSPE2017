using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using System.Transactions;
using LinqKit;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAgendaActividadLibertadDetalle : EntityManagerServer<AGENDA_ACT_LIB_DETALLE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAgendaActividadLibertadDetalle()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<AGENDA_ACT_LIB_DETALLE> ObtenerTodos(List<decimal> Id = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<AGENDA_ACT_LIB_DETALLE>();
                if (Id != null)
                    predicate = predicate.And(w => Id.Contains(w.ID_AGENDA_ACTIVIDAD_LIBERTAD));
                return GetData(predicate.Expand()).OrderBy(w => w.FECHA);
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
        public AGENDA_ACT_LIB_DETALLE Obtener(int Id, int IdDetalle)
        {
            try
            {
                return GetData().FirstOrDefault(w => w.ID_AGENDA_ACTIVIDAD_LIBERTAD == Id && w.ID_DETALLE == IdDetalle);
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
        public decimal Insertar(AGENDA_ACT_LIB_DETALLE Entity)
        {
            try
            {
                Entity.ID_DETALLE = GetIDProceso<short>("AGENDA_ACT_LIB_DETALLE", "ID_DETALLE", string.Format("ID_AGENDA_ACTIVIDAD_LIBERTAD = {0}",Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD));
                if(Insert(Entity))
                    return Entity.ID_AGENDA_ACTIVIDAD_LIBERTAD;
                return 0;
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
        public bool Actualizar(AGENDA_ACT_LIB_DETALLE Entity,List<AGENDA_ACT_LIB_DETALLE> Lista)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    Context.Entry(Entity).State = System.Data.EntityState.Modified;

                    if(Lista != null)
                    {
                        foreach (var l in Lista)
                        {
                            Context.Entry(l).State = System.Data.EntityState.Modified;                   
                        }
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(AGENDA_ACT_LIB_DETALLE Entity)
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