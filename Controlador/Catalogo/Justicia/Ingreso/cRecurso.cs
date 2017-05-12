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


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cRecurso : EntityManagerServer<RECURSO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cRecurso()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "RECURSO"</returns>
        public IQueryable<RECURSO> ObtenerTodos(short Centro, short Anio, int Imputado, short Ingreso, short CausaPenal)
        {
            try
            {
                return GetData().Where(w => w.ID_ANIO == Anio && w.ID_CENTRO == Centro && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal);
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
        /// <returns>objeto de tipo "RECURSO"</returns>
        public List<RECURSO> Obtener(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal)
        {
            var Resultado = new List<RECURSO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "RECURSO" con valores a insertar</param>
        public short Insertar(RECURSO Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    #region revisamos el resultado
                    /*REVOCA CON REPOSICION DE PROCEDIMIENTO*/
                    if ((Entity.ID_TIPO_RECURSO == 2 && Entity.RESULTADO == "3") || (Entity.ID_TIPO_RECURSO == 3 && Entity.RESULTADO == "3"))
                    {
                        var cp = Context.CAUSA_PENAL.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL).FirstOrDefault();
                        if (cp != null)
                        {
                            cp.ID_ESTATUS_CP = 6;
                            Context.Entry(cp).State = System.Data.EntityState.Modified;

                            var sentencia = Context.SENTENCIA.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL && w.ESTATUS == "A").FirstOrDefault();
                            if (sentencia != null)
                            {
                                sentencia.ESTATUS = "I";
                                Entity.ID_SENTENCIA = sentencia.ID_SENTENCIA;
                                Context.Entry(sentencia).State = System.Data.EntityState.Modified;
                            }
                        }
                    }
                    if ((Entity.ID_TIPO_RECURSO == 2 && Entity.RESULTADO == "2"))
                    {
                        var sentencia = Context.SENTENCIA.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL && w.ESTATUS == "A").FirstOrDefault();
                        if (sentencia != null)
                        {
                            Entity.ID_SENTENCIA = sentencia.ID_SENTENCIA;
                        }
                    }
                    #endregion

                    Entity.ID_RECURSO = GetIDProceso<short>("RECURSO", "ID_RECURSO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_CAUSA_PENAL = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_CAUSA_PENAL));
                    Context.RECURSO.Add(Entity);

                    Context.SaveChanges();
                    transaccion.Complete();
                    return Entity.ID_RECURSO;
                }
                //Entity.ID_RECURSO = GetIDProceso<short>("RECURSO", "ID_RECURSO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_CAUSA_PENAL = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_CAUSA_PENAL));
                //if(Insert(Entity))               
                //    return Entity.ID_RECURSO;
                //else
                //    return 0;
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
        /// <param name="Entity">objeto de tipo "RECURSO" con valores a actualizar</param>
        public bool Actualizar(RECURSO Entity,List<RECURSO_DELITO> Delito)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = System.Data.EntityState.Modified;
                    #region Delitos
                    var delitos = Context.RECURSO_DELITO.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL && w.ID_RECURSO == Entity.ID_RECURSO);
                    if (delitos != null)
                    {
                        foreach (var d in delitos)
                        {
                            Context.Entry(d).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var d in Delito)
                    {
                        Context.RECURSO_DELITO.Add(d);
                    }
                    #endregion

                    #region revisamos el resultado
                    /*REVOCA CON REPOSICION DE PROCEDIMIENTO*/
                    if (Entity.ID_TIPO_RECURSO == 2 && Entity.RESULTADO == "3")
                    {
                        var cp = Context.CAUSA_PENAL.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL).FirstOrDefault();
                        if (cp != null)
                        {
                            cp.ID_ESTATUS_CP = 6;
                            Context.Entry(cp).State = System.Data.EntityState.Modified;

                            var sentencia = Context.SENTENCIA.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL && w.ESTATUS == "A").FirstOrDefault();
                            if (sentencia != null)
                            {
                                sentencia.ESTATUS = "I";
                                Context.Entry(sentencia).State = System.Data.EntityState.Modified;
                            }
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //Update(Entity);
                //return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal, int Recurso)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_RECURSO == Recurso).ToList();
                if (Entity != null)
                {
                    (new cRecursoDelito()).EliminarMultiple(Centro, Anio, Imputado, Ingreso, CausaPenal, Recurso);//eliminamos los delitos de la causa penal
                    return Delete(Entity); 
                }
                else
                    return false;
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