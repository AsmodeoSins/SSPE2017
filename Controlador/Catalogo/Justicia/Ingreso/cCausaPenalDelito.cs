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
    public class cCausaPenalDelito : EntityManagerServer<CAUSA_PENAL_DELITO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCausaPenalDelito()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "CAUSA_PENAL_DELITO"</returns>
        public IQueryable<CAUSA_PENAL_DELITO> ObtenerTodos()
        {
            try
            {
                return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<CAUSA_PENAL_DELITO> ObtenerTodos(short Centro = 0, short Anio = 0, int Imputado = 0, short Ingreso = 0, short CausaPenal = 0)
        {
            try
            {
                if(Centro > 0)
                    return  GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal);
                else
                    return GetData();
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
        /// <returns>objeto de tipo "CAUSA_PENAL_DELITO"</returns>
        public List<CAUSA_PENAL_DELITO> Obtener(int Centro, int Anio, int Imputado, int Ingreso)
        {
            var Resultado = new List<CAUSA_PENAL_DELITO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso).ToList();
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
        /// <param name="Entity">objeto de tipo "CAUSA_PENAL_DELITO" con valores a insertar</param>
        public int Insertar(CAUSA_PENAL_DELITO Entity)
        {
            try
            {
                //Entity.ID_IMPUTADO = GetSequence<short>("IMPUTADO_SEQ");
                Insert(Entity);
                return Entity.ID_IMPUTADO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return 0;
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "CAUSA_PENAL_DELITO" con valores a insertar</param>
        public bool Insertar(short Centro, short Anio, int Imputado, short Ingreso, short CausaPenal, short Sentencia, List<CAUSA_PENAL_DELITO> DelitoCP, List<SENTENCIA_DELITO> DelitoS)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    #region Delito Causa Penal
                    var delitosCP = Context.CAUSA_PENAL_DELITO.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal);
                    if (delitosCP != null)
                    {
                        foreach (var d in delitosCP)
                        {
                            Context.Entry(d).State = System.Data.EntityState.Deleted;
                        }
                    }
                    #endregion
                    #region Delito Sentencia
                    var delitosS = Context.SENTENCIA_DELITO.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_SENTENCIA == Sentencia);
                    if (delitosS != null)
                    {
                        foreach (var d in delitosS)
                        {
                            Context.Entry(d).State = System.Data.EntityState.Deleted;
                        }
                    }
                    #endregion

                    foreach (var d in DelitoCP)
                    {
                        Context.CAUSA_PENAL_DELITO.Add(d);
                    }

                    foreach (var d in DelitoS)
                    {
                        Context.SENTENCIA_DELITO.Add(d);
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                    
                }

                //EliminarMultiple(Centro, Anio, Imputado, Ingreso, CausaPenal);
                //Insert(Delitos);
                //return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;

        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "CAUSA_PENAL_DELITO" con valores a actualizar</param>
        public bool Actualizar(CAUSA_PENAL_DELITO Entity)
        {
            try
            {
                Update(Entity);
                return true;
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
        public bool Eliminar(int Centro, int Anio, int Imputado, int Ingreso,int Delito)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_DELITO == Delito).ToList();
                if (Entity != null)
                    return Delete(Entity);
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


          /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool EliminarMultiple(int Centro, int Anio, int Imputado, int Ingreso,int CausaPenal)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal).ToList();
                if (Entity != null)
                {
                    foreach (CAUSA_PENAL_DELITO delito in Entity)
                    {
                        Delete(delito);
                    }
                    return true;
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