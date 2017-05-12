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
    public class cProgramaLibertad : EntityManagerServer<PROGRAMA_LIBERTAD>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cProgramaLibertad()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "FUERO"</returns>
        public IQueryable<PROGRAMA_LIBERTAD> ObtenerTodos(string Buscar = "",string Estatus = "A")
        {
            try
            {
                var predicate = PredicateBuilder.True<PROGRAMA_LIBERTAD>();
                if (!string.IsNullOrEmpty(Buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(Buscar));
                if (!string.IsNullOrEmpty(Estatus))
                    predicate = predicate.And(w => w.ESTATUS == Estatus);
                return GetData(predicate.Expand()).OrderBy(w => w.DESCR);
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
        public PROGRAMA_LIBERTAD Obtener(short Id)
        {
            try
            {
                return GetData().FirstOrDefault(w => w.ID_PROGRAMA_LIBERTAD == Id);
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
        public decimal Insertar(PROGRAMA_LIBERTAD Entity)
        {
            try
            {
                Entity.ID_PROGRAMA_LIBERTAD = GetIDProceso<short>("PROGRAMA_LIBERTAD", "ID_PROGRAMA_LIBERTAD", "1=1");
                if(Insert(Entity))
                    return Entity.ID_PROGRAMA_LIBERTAD;
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
        public bool Actualizar(PROGRAMA_LIBERTAD Entity, List<ACTIVIDAD_PROGRAMA> Actividades)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    Context.Entry(Entity).State = System.Data.EntityState.Modified;
                    //Responsables
                    var res = Context.ACTIVIDAD_PROGRAMA.Where(w => w.ID_PROGRAMA_LIBERTAD == Entity.ID_PROGRAMA_LIBERTAD);
                    if (res != null)
                        foreach (var r in res)
                            Context.Entry(r).State = System.Data.EntityState.Deleted;
                    if (Actividades != null)
                    {
                        short i = 1;
                        foreach (var r in Actividades)
                        {
                            r.ID_PROGRAMA_LIBERTAD = Entity.ID_PROGRAMA_LIBERTAD;
                            r.ID_ACTIVIDAD_PROGRAMA = i;
                            Context.Entry(r).State = System.Data.EntityState.Added;
                            i++;
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
        public bool Eliminar(PROGRAMA_LIBERTAD Entity)
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