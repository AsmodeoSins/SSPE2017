using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Configuration;
using System.Data.EntityClient;


namespace SSP.Modelo
{
    public class EntityManagerServer<TEntity> where TEntity : class
    {
        #region Global Variables
        private SSPEntidades context;
        public SSPEntidades Context
        {
            get { return context; }
        }
        //internal ObjectSet<TEntity> ObjectSet;
        protected DbSet<TEntity> DbSet;
        #endregion Global Variables
        #region Constructor
        /// <summary>
        /// Constructor que inicializa el contexto con un otro ya existente
        /// </summary>
        /// <param name="contexto">contexto necesario para inicializar otro</param>
        public EntityManagerServer(SSPEntidades contexto)
        {
            this.context = contexto;
            this.context.Conexion();
            this.DbSet = this.context.Set<TEntity>();//CreateObjectSet<TEntity>();
        }
        /// <summary>
        /// Constructor que inicializa el contexto con los datos predefinidos
        /// </summary>
        public EntityManagerServer()
        {
            //this.context = new SSPEntidades();
            //this.DbSet = this.context.Set<TEntity>();//CreateObjectSet<TEntity>();
            this.getDbSet();
        }

        ///<summary>
        ///actualiza el listado de los datos
        ///</summary>
        public void getDbSet()
        {
            this.context = new SSPEntidades();
            this.context.Conexion();
            this.DbSet = this.context.Set<TEntity>();//CreateObjectSet<TEntity>();
        }
        /// <summary>
        /// Constructor que inicializa el contexto con los datos predefinidos
        /// </summary>
        /*public EntityManagerServer(string nameOrConnectionString)
        {
            this.context = new SC3Entities(nameOrConnectionString);
            this.DbSet = this.context.Set<TEntity>();//CreateObjectSet<TEntity>();
        }*/
        #endregion Constructor
        #region Methods
        void Actualiza()
        {
            this.context = new SSPEntidades();
            this.context.Conexion();
            this.DbSet = this.context.Set<TEntity>();//CreateObjectSet<TEntity>();
        }


        public bool GuardarConfiguracionEntidad(string server, string user, string password, string database)
        {
            try
            {
                //<add name="SSPEntidades" connectionString="metadata=res://*/Modelo.csdl|res://*/Modelo.ssdl|res://*/Modelo.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string=&quot;DATA SOURCE=SSPE;PASSWORD=QUADRO;USER ID=SYSTEM&quot;" providerName="System.Data.EntityClient" />
                
                //Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                //config.ConnectionStrings.ConnectionStrings["SSPEntidades"].ConnectionString = string.Format("metadata=res://*/Modelo.csdl|res://*/Modelo.ssdl|res://*/Modelo.msl;provider=Oracle.ManagedDataAccess.Client;provider connection string=&quot;DATA SOURCE={0};PASSWORD={1};USER ID={2}&quot;", database, password, user);
                //config.ConnectionStrings.ConnectionStrings["SSPEntidades"].ProviderName = "System.Data.EntityClient";
                //config.Save(ConfigurationSaveMode.Full, true);
                //ConfigurationManager.RefreshSection("connectionStrings");
                return true;
            }

            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene la información del objeto que se mande según el id que reciba
        /// </summary>
        /// <param name="id">id a buscar</param>
        public virtual TEntity GetSingle(Func<TEntity, bool> filter)
        {
            return DbSet.SingleOrDefault(filter);
        }
        /// <summary>
        /// Obtiene la información del objeto que sele mande
        /// </summary>
        /// <param name="filter">filtros que de aplicaran en la búsqueda</param>
        /// <param name="orderBy">ordenamiento de la información</param>
        /// <param name="includeProperties">Nombre de los objetos que se incluiran en la busqueda</param>
        public virtual IQueryable<TEntity> GetData(
            System.Linq.Expressions.Expression<Func<TEntity, bool>> filter = null,
                Func<TEntity, string> orderBy = null,
                    string includeProperties = "")
        {
            IQueryable<TEntity> qQuery;
            if (filter != null)
                qQuery = DbSet.Where(filter).AsQueryable();
            else
                qQuery = DbSet;
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    qQuery = qQuery.Include(includeProperty.Trim());
            }
            return qQuery;
        }
        /// <summary>
        /// Agrega la información del objeto que se mande
        /// </summary>
        /// <param name="entity">Objeto a guardar</param>        
        public virtual bool Insert(TEntity entity)
        {
                try
                {
                    DbSet.Add(entity);
                    if (!context.SaveChanges().Equals(0))
                    {
                        return true;
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                return false;
        }

        public virtual bool Insert(TEntity entity,SSPEntidades Contexto)
        {
            try
            {
                DbSet<TEntity> dbSet;
                dbSet = Contexto.Set<TEntity>();
                dbSet.Add(entity);
                if (!Contexto.SaveChanges().Equals(0))
                {
                    return true;
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Agrega la información del objeto que se mande
        /// </summary>
        /// <param name="entity">Objeto a guardar</param>        
        public virtual bool Insert(TEntity entity, out TEntity entidad)
        {
            //using (var transaction = new TransactionScope())
            //{
                try
                {
                    entidad = DbSet.Add(entity);
                    if (!context.SaveChanges().Equals(0))
                    {
                        //transaction.Complete();
                        //transaction.Dispose();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                //transaction.Dispose();
                return false;
            //}
        }
        /// <summary>
        /// Inserta un listado de entidades si marca error automaticamente no se insertan los otros registros (transacciones)
        /// </summary>
        /// <param name="lstEntities"></param>
        /// <returns></returns>
        public virtual bool Insert(List<TEntity> lstEntities)
        {
            //using (var transaction = new TransactionScope())
            //{
                try
                {
                    foreach (var entity in lstEntities)
                    {
                        DbSet.Add(entity);
                    }

                    if (!context.SaveChanges().Equals(0))
                    {
                        //transaction.Complete();
                        //transaction.Dispose();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                //transaction.Dispose();
                return false;
            //}
        }
        /// <summary>
        /// Metodo que retorna la secuencia de determinada base de datos por ejemplo (oracle, postgresql)
        /// </summary>
        /// <returns></returns>
        public virtual T GetSequence<T>(string sequence) where T : struct
        {
            return context.Database.SqlQuery<T>(string.Format("select SSP.{0}.nextval from dual", sequence)).Single<T>();
        }

        /// <summary>
        /// Metodo que retorna la secuencia de determinada base de datos por ejemplo (oracle, postgresql)
        /// </summary>
        /// <returns></returns>
        public virtual T GetIDCatalogo<T>(string table) where T : struct
        {
            string query = string.Format("SELECT NVL(MAX(ID_{0}),99)+1 AS ID FROM SSP.{0} WHERE ID_{0} > 99 AND ID_{0} < 9999", table);
            return context.Database.SqlQuery<T>(query).Single<T>();
        }

        /// <summary>
        /// Metodo que retorna la secuencia de determinada base de datos por ejemplo (oracle, postgresql)
        /// </summary>
        /// <returns></returns>
        public virtual T GetIDProceso<T>(string tabla, string columna, string filtros) where T : struct
        {
            string query = string.Format("SELECT NVL(MAX({0}),0) + 1 AS ID FROM SSP.{1} WHERE {2} ", columna, tabla, filtros);
            return context.Database.SqlQuery<T>(query).Single<T>();
        }

        /// <summary>
        /// Metodo que ejecuta queries
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetEjecutaQueries<T>(string query)  
        {
            return context.Database.SqlQuery<T>(query).ToList();
        }

        /// <summary>
        /// Obtener resultado de query (single)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual T GetEjecutaSingleQueries<T>(string query)
        {
            return context.Database.SqlQuery<T>(query).Single<T>();
        }

        public virtual T GetIDProceso<T>(string tabla, string columna, string filtros, SSPEntidades Contexto) where T : struct
        {
            string query = string.Format("SELECT NVL(MAX({0}),0) + 1 AS ID FROM SSP.{1} WHERE {2} ", columna, tabla, filtros);
            return Contexto.Database.SqlQuery<T>(query).Single<T>();
        }
        /// <summary>
        /// Metodo que retorna la hora y fecha del servidor
        /// </summary>
        /// <returns></returns>
        public virtual string GetFechaServer()
        {
            //return context.Database.SqlQuery<string>(string.Format("SELECT TO_CHAR(sysdate, 'DD/MM/YYYY HH:MI:SS AM') FROM dual")).SingleOrDefault();
            return context.Database.SqlQuery<string>("SELECT TO_CHAR(SYSDATE, 'DD/MM/YYYY HH:MI:SS AM') FROM DUAL").SingleOrDefault();
        }

        public virtual DateTime GetFechaServerDate()
        {
            //return context.Database.SqlQuery<string>(string.Format("SELECT TO_CHAR(sysdate, 'DD/MM/YYYY HH:MI:SS AM') FROM dual")).SingleOrDefault();
            return context.Database.SqlQuery<DateTime>("SELECT SYSDATE FROM DUAL").SingleOrDefault();
        }

        //Crear usuario BD
        /// <summary>
        /// Metodo para  crear un usuario de base de datos
        /// </summary>
        /// <returns></returns>
        public virtual int CreateUserDB(string User, string Pass)
        {
            return context.CREARUSUARIO(User,Pass);
        }

        //Cambiar password usuario BD
        /// <summary>
        /// Metodo para cambiar el password de un usuario de base de datos
        /// </summary>
        /// <returns></returns>
        public virtual int UpdatePassUserDB(string User, string Pass)
        {
            return context.CAMBIAPASSWORD(User, Pass);
        }

        [Obsolete]
        public virtual void Open()
        {
            if (context.Database.Connection.State != ConnectionState.Open)
                context.Database.Connection.Open();
        }
        /// <summary>
        /// Actualizar la información del objeto que se mande, se necesita todos los parametros
        /// </summary>
        /// <param name="entityToUpdate">Objeto a Actualizar</param>
        public virtual bool Update(TEntity entityToUpdate)
        {
            //using (var transaction = new TransactionScope())
            //{
                try
                {   

                    SSPEntidades contexto = new SSPEntidades();
                    contexto.Conexion();
                    contexto.Entry(entityToUpdate).State = EntityState.Modified;
                        if (!contexto.SaveChanges().Equals(0))
                    {
                        //transaction.Complete();
                        //transaction.Dispose();
                        return true;
                    }
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                //transaction.Dispose();
                return false;
            //}
        }
        /// <summary>
        /// Actualizar la información del objeto que se mande
        /// </summary>
        /// <param name="entityToUpdate">Objeto a Actualizar</param>
        public virtual bool Update(List<TEntity> entityToUpdate)
        {
            //using (var transaction = new TransactionScope())
            //{
                try
                {
                    foreach (var item in entityToUpdate)
                    {
                        DbSet.Attach(item);
                        context.Entry(item).State = EntityState.Modified;
                    }

                    if (!context.SaveChanges().Equals(0))
                    {
                        //transaction.Complete();
                        //transaction.Dispose();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                //transaction.Dispose();
                return false;
            //}
        }
        /// <summary>
        /// Borra el objeto que concuerde con el id
        /// </summary>
        /// <param name="id">id del objeto a borrar</param>
        public virtual bool Delete(Func<TEntity, bool> filter)
        {
            TEntity entityToDelete = GetSingle(filter);
            return Delete(entityToDelete);
        }
        /// <summary>
        /// Borra la información del objeto que se mande
        /// </summary>
        /// <param name="entityToDelete">Objeto a Borrar</param>
        public virtual bool Delete(TEntity entityToDelete)
        {
            //using (var transaction = new TransactionScope())
            //{
                try

                {
                    context.Entry(entityToDelete).State = EntityState.Deleted;
                    DbSet.Remove(entityToDelete);
                    if (!context.SaveChanges().Equals(0))
                    {
                        //transaction.Complete();
                        //transaction.Dispose();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.InnerException.ToString());
                }
                //transaction.Dispose();
                return false;
            //}
        }
        /// <summary>
        /// Borra los registros de un listado determinado
        /// </summary>
        /// <param name="entityToDelete"></param>
        /// <returns></returns>
        public virtual bool Delete(List<TEntity> entityToDelete)
        {
            //using (var transaction = new TransactionScope())
            //{
                try
                {
                    foreach (var item in entityToDelete)
                    {
                        DbSet.Attach(item);
                        context.Entry(item).State = EntityState.Deleted;
                    }
                    if (!context.SaveChanges().Equals(0))
                    {
                        //transaction.Complete();
                        //transaction.Dispose();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                //transaction.Dispose();
                return false;
            //}
        }
        /// <summary>
        /// Concatena 2 filtros del mismo tipo con expresion and (&&)
        /// </summary>
        /// <param name="funcIzq">primer filtro a validar</param>
        /// <param name="FuncDer">segundo filtro a validar</param>
        /// <returns>resultado de la concatenacion de los filtros</returns>
        public virtual Func<TEntity, bool> ConcatenarFiltros(Func<TEntity, bool> funcIzq, Func<TEntity, bool> FuncDer)
        {
            return tEntity => funcIzq(tEntity) && FuncDer(tEntity);
        }
        /// <summary>
        /// Concatena varios filtros del mismo tipo con expresion and (&&)
        /// </summary>
        /// <param name="funcs">lista de filtros a concatenar</param>
        /// <returns>resultado de la concatenacion de los filtros</returns>
        public virtual Func<TEntity, bool> ConcatenarFiltros(List<Func<TEntity, bool>> funcs)
        {
            Func<TEntity, bool> funcConcatenada = tEntity => true;
            foreach (Func<TEntity, bool> func in funcs)
                funcConcatenada = ConcatenarFiltros(funcConcatenada, func);
            return funcConcatenada;
        }
        #endregion Methods
        /// <summary>
        /// Metodo que regresa informacion de tipo IEnumerable, no es recomendable utilizarlo
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetDataEnumerable(
          Func<TEntity, bool> filter = null,
          Func<TEntity, string> orderBy = null,
          string includeProperties = "")
        {
            IQueryable<TEntity> qQuery;
            if (filter != null)
                qQuery = DbSet.Where(filter).AsQueryable();
            else
                qQuery = DbSet;
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    qQuery = qQuery.Include(includeProperty.Trim());
            }
            if (orderBy != null)
                return qQuery.OrderBy(orderBy).ToList();
            else
                return qQuery.ToList();
        }
        public virtual IEnumerable<TEntity> GetDataM(
             Func<TEntity, bool> filter = null,
             Func<TEntity, string> orderBy = null,
             string includeProperties = "")
        {
            IQueryable<TEntity> qQuery;
            if (filter != null)
                qQuery = DbSet.Where(filter).AsQueryable();
            else
                qQuery = DbSet;
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    qQuery = qQuery.Include(includeProperty.Trim());
            }
            if (orderBy != null)
                return qQuery.OrderBy(orderBy).ToList();
            else
                return qQuery.ToList();
        }
        /// <summary>
        /// Metodo que retorna true si existe la base de datos
        /// </summary>
        /// <returns></returns>
        public bool Exists()
        {
            return context.Database.Exists();
        }
    }
}