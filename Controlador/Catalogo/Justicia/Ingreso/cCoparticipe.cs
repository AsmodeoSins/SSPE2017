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
    public class cCoparticipe : EntityManagerServer<COPARTICIPE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCoparticipe()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "COPARTICIPE"</returns>
        public ObservableCollection<COPARTICIPE> ObtenerTodos()
        {
            ObservableCollection<COPARTICIPE> coparticipe;
            var Resultado = new List<COPARTICIPE>();
            try
            {
                Resultado = GetData().ToList();
                coparticipe = new ObservableCollection<COPARTICIPE>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return coparticipe;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "COPARTICIPE"</returns>
        public List<COPARTICIPE> Obtener(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal)
        {
            var Resultado = new List<COPARTICIPE>();
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
        /// <param name="Entity">objeto de tipo "COPARTICIPE" con valores a insertar</param>
        public long Insertar(COPARTICIPE Entity)
        {
            try
            {
                Insert(Entity);
                return  Entity.ID_COPARTICIPE;
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
        /// <param name="Entity">objeto de tipo "COPARTICIPE" con valores a insertar</param>
        public bool Insertar(int Centro,int Anio,int Imputado,int Ingreso, int CausaPenal,List<COPARTICIPE> listaCoparticipes)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    #region Alias
                    var alias = Context.COPARTICIPE_ALIAS.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal);
                    if (alias != null)
                    {
                        foreach (var a in alias)
                        {
                            Context.Entry(a).State = System.Data.EntityState.Deleted;
                        }
                    }
                    #endregion
                    #region Apodo
                    var apodos = Context.COPARTICIPE_APODO.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal);
                    if (apodos != null)
                    {
                        foreach (var a in apodos)
                        {
                            Context.Entry(a).State = System.Data.EntityState.Deleted;
                        }
                    }
                    #endregion
                    #region Coparticipes
                    var coparticipes = Context.COPARTICIPEs.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal);
                    if (coparticipes != null)
                    {
                        foreach (var c in coparticipes)
                        {
                            Context.Entry(c).State = System.Data.EntityState.Deleted;
                        }
                    }
                    #endregion
                    foreach (var c in listaCoparticipes)
                    {
                        Context.COPARTICIPEs.Add(c);
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //Eliminar(Centro, Anio, Imputado, Ingreso, CausaPenal);
                //if (listaCoparticipes.Count == 0)
                //    return true;
                //else
                //    if (Insert(listaCoparticipes))
                //        return true;
                //    else
                //        return false;
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
        /// <param name="Entity">objeto de tipo "MODALIDAD_DELITO" con valores a actualizar</param>
        public bool Actualizar(COPARTICIPE Entity)
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
        public bool Eliminar(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal,int Coparticipe)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_COPARTICIPE == Coparticipe).ToList();
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
        public bool Eliminar(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal).ToList();
                if (Entity != null)
                {
                    (new cCoparticipeAlias()).EliminarObtener(Centro, Anio, Imputado, Ingreso, CausaPenal);
                    (new cCoparticipeApodo()).EliminarObtener(Centro, Anio, Imputado, Ingreso, CausaPenal);
                    foreach (COPARTICIPE coparticipe in Entity)
                    {
                        Delete(coparticipe);
                    }
                    return true;
                }
                //if (Entity != null)
                //    return Delete(Entity);
                //else
                //    return false;
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