using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using LinqKit;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTipoMensaje : EntityManagerServer<MENSAJE_TIPO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cTipoMensaje()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "APODO"</returns>
        public IQueryable<MENSAJE_TIPO> ObtenerTodos(string buscar = "", string estatus = "S" )
        {
            try
            {
                var predicate = PredicateBuilder.True<MENSAJE_TIPO>();
                if (!string.IsNullOrEmpty(buscar))
                    predicate = predicate.And(w => w.DESCR.Contains(buscar) || w.ENCABEZADO.Contains(buscar) || w.CONTENIDO.Contains(buscar));
                if(!string.IsNullOrEmpty(estatus))
                    predicate = predicate.And(w => w.ESTATUS == estatus);
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
        /// <returns>objeto de tipo "APODO"</returns>
        public IQueryable<MENSAJE_TIPO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_MEN_TIPO == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ALIAS" con valores a insertar</param>
        public short Agregar(MENSAJE_TIPO Entity)
        {
            try
            {
                Entity.ID_MEN_TIPO = GetIDProceso<short>("MENSAJE_TIPO","ID_MEN_TIPO", "1 = 1");
                if (Insert(Entity))
                    return Entity.ID_MEN_TIPO;
                else
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
        /// <param name="Entity">objeto de tipo "APODO" con valores a actualizar</param>
        public bool Actualizar(MENSAJE_TIPO Entity,List<MENSAJE_ROL> lista)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = System.Data.EntityState.Modified;
                    
                    #region Listas
                    var roles = Context.MENSAJE_ROL.Where(w => w.ID_MEN_TIPO == Entity.ID_MEN_TIPO);
                    if (roles != null)
                    {
                        foreach (var r in roles)
                        {
                            Context.Entry(r).State = System.Data.EntityState.Deleted;
                        }
                    }

                    if (lista != null)
                    {
                        foreach (var r in lista)
                        {
                            Context.MENSAJE_ROL.Add(r);
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                /*if (Update(Entity))
                    return true;
                else
                    return false;*/
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
        public bool Eliminar(MENSAJE_TIPO Entity)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_MEN_TIPO == Entity.ID_MEN_TIPO);//.SingleOrDefault();
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
    }
}