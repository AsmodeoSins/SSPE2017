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
    public class cUnidadReceptora : EntityManagerServer<UNIDAD_RECEPTORA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cUnidadReceptora()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<UNIDAD_RECEPTORA> ObtenerTodos(string Buscar = "",string Estatus = "A")
        {
            try
            {
                var predicate = PredicateBuilder.True<UNIDAD_RECEPTORA>();
                if (!string.IsNullOrEmpty(Buscar))
                    predicate = predicate.And(w => w.NOMBRE.Contains(Buscar));
                if (!string.IsNullOrEmpty(Estatus))
                    predicate = predicate.And(w => w.ESTATUS == Estatus);
                return GetData(predicate.Expand()).OrderBy(w => w.DESCRIPCION);
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
        public UNIDAD_RECEPTORA Obtener(short Id)
        {
            try
            {
                return GetData().FirstOrDefault(w => w.ID_UNIDAD_RECEPTORA == Id);
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
        public decimal Insertar(UNIDAD_RECEPTORA Entity,List<UNIDAD_RECEPTORA_RESPONSABLE> Responsables)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    Entity.ID_UNIDAD_RECEPTORA = GetIDProceso<short>("UNIDAD_RECEPTORA", "ID_UNIDAD_RECEPTORA", "1=1");
                    Context.Entry(Entity).State = System.Data.EntityState.Added;
                    //Responsables
                    short i = 1;
                    if (Responsables != null)
                    {
                        foreach (var r in Responsables)
                        {
                            r.ID_UNIDAD_RECEPTORA = Entity.ID_UNIDAD_RECEPTORA;
                            r.ID_UNIDAD_RECEPTORA_RES = i;
                            Context.Entry(r).State = System.Data.EntityState.Added;
                            i++;
                        }
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_UNIDAD_RECEPTORA;
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
        public bool Actualizar(UNIDAD_RECEPTORA Entity,List<UNIDAD_RECEPTORA_RESPONSABLE> Responsables)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    Context.Entry(Entity).State = System.Data.EntityState.Modified;
                    //Responsables
                    var res = Context.UNIDAD_RECEPTORA_RESPONSABLE.Where(w => w.ID_UNIDAD_RECEPTORA == Entity.ID_UNIDAD_RECEPTORA);
                    if (res != null)
                        foreach (var r in res)
                            Context.Entry(r).State = System.Data.EntityState.Deleted;
                    if (Responsables != null)
                        foreach (var r in Responsables)
                            Context.Entry(r).State = System.Data.EntityState.Added;
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
        public bool Eliminar(UNIDAD_RECEPTORA Entity)
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