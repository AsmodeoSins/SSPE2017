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


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cSentencia : EntityManagerServer<SENTENCIA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cSentencia()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "SENTENCIA"</returns>
        public ObservableCollection<SENTENCIA> ObtenerTodos()
        {
            ObservableCollection<SENTENCIA> sentencia;
            var Resultado = new List<SENTENCIA>();
            try
            {
                Resultado = GetData().ToList();
                sentencia = new ObservableCollection<SENTENCIA>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return sentencia;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "SENTENCIA"</returns>
        public IQueryable<SENTENCIA> Obtener(short? Centro = null,short? Anio = null,int? Imputado = null,short? Ingreso = null,short? CausaPenal = null,short? Sentencia = null,string Estatus = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<SENTENCIA>();
                if (Centro == null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio == null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado== null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Ingreso == null)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);
                if (CausaPenal == null)
                    predicate = predicate.And(w => w.ID_CAUSA_PENAL == CausaPenal);
                if (Sentencia == null)
                    predicate = predicate.And(w => w.ID_SENTENCIA == Sentencia);
                if(!string.IsNullOrEmpty(Estatus))
                    predicate = predicate.And(w => w.ESTATUS == Estatus);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "SENTENCIA" con valores a insertar</param>
        public short Insertar(SENTENCIA Entity)
        {
            try
            {
                Entity.ID_SENTENCIA = GetIDProceso<short>("SENTENCIA", "ID_SENTENCIA", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND  ID_INGRESO = {3} AND ID_CAUSA_PENAL = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO,Entity.ID_CAUSA_PENAL));
                Insert(Entity);
                #region cambio estatus juridico
                var ingreso = new INGRESO();
                ingreso.ID_CENTRO = Entity.ID_CENTRO;
                ingreso.ID_ANIO = Entity.ID_ANIO;
                ingreso.ID_IMPUTADO = Entity.ID_IMPUTADO;
                ingreso.ID_INGRESO = Entity.ID_INGRESO;
                ingreso.ID_CLASIFICACION_JURIDICA = "3";//SENTENCIADO
                Context.INGRESO.Attach(ingreso);
                Context.Entry(ingreso).Property(x => x.ID_CLASIFICACION_JURIDICA).IsModified = true;
                #endregion
                return  Entity.ID_SENTENCIA;
            }
            catch (Exception ex)
            {
                return 0;
                //throw new ApplicationException(ex.Message);
            }
            
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "SENTENCIA" con valores a actualizar</param>
        public bool Actualizar(SENTENCIA Entity,List<SENTENCIA_DELITO> Delito)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = System.Data.EntityState.Modified;
                    #region Delitos
                    var delitos = Context.SENTENCIA_DELITO.Where(w => w.ID_CENTRO == Entity.ID_CENTRO && w.ID_ANIO == Entity.ID_ANIO && w.ID_IMPUTADO == Entity.ID_IMPUTADO && w.ID_INGRESO == Entity.ID_INGRESO && w.ID_CAUSA_PENAL == Entity.ID_CAUSA_PENAL && w.ID_SENTENCIA == Entity.ID_SENTENCIA);
                    if (delitos != null)
                    {
                        foreach (var d in delitos)
                        {
                            Context.Entry(d).State = System.Data.EntityState.Deleted;
                        }
                    }
                    foreach (var d in Delito)
                    {
                        Context.SENTENCIA_DELITO.Add(d);
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
               throw new ApplicationException(ex.Message);
            }
            return false;
        }

        public bool ActualizarEstatusSentencia(SENTENCIA Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.SENTENCIA.Attach(Entity);
                    Context.Entry(Entity).Property(x => x.ESTATUS).IsModified = true;
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
        public bool Eliminar(int Centro, int Anio, int Imputado, int Ingreso, int CausaPenal, int Sentencia)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso && w.ID_CAUSA_PENAL == CausaPenal && w.ID_SENTENCIA == Sentencia).ToList();
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
        /// Metodo que obtiene la sentencia de un ingreso
        /// </summary>
        /// <param name="Centro"></param>
        /// <param name="Anio"></param>
        /// <param name="Imputado"></param>
        /// <param name="Ingreso"></param>
        /// <param name="Fecha"></param>
        /// <returns></returns>
        public IEnumerable<Sentencia> ObtenerSentenciaIngreso(short Centro,short Anio,int Imputado,short Ingreso,DateTime Fecha)
        {
            try
            {
                string query = string.Format("SELECT T.* FROM  TABLE(SSP.SENTENCIA_FUNC({0},{1},{2},{3},TO_DATE('{4}','{5}'))) T", Centro, Anio, Imputado, Ingreso, Fecha.ToShortDateString(), "dd/mm/yyyy");
                return Context.Database.SqlQuery<Sentencia>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public class Sentencia 
        {
            public DateTime? FECHA_INICIO { get; set; }
            public int S_ANIO { get; set; }
            public int S_MES { get; set; }
            public int S_DIA { get; set; }
            public int C_ANIO { get; set; }
            public int C_MES { get; set; }
            public int C_DIA { get; set; }
            public int A_ANIO { get; set; }
            public int A_MES { get; set; }
            public int A_DIA { get; set; }
            public int PC_ANIO { get; set; }
            public int PC_MES { get; set; }
            public int PC_DIA { get; set; }
        }
        
    }
}