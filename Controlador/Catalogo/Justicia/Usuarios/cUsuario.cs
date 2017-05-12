using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;
using System.Transactions;
using System.Data;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cUsuario : EntityManagerServer<USUARIO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cUsuario()
        { }


        /// <summary>
        /// Obtiene todos los usuarios de un area tecnica de un centro
        /// </summary>
        /// <param name="id_centro">ID del centro donde se hace la consulta</param>
        /// <param name="area_tecnica">ID del area tecnica</param>
        /// <param name="id_roles">Lista de ID de roles de los usuarios</param>
        /// <returns></returns>
        public IQueryable<USUARIO> ObtenerTodosporAreaTecnica(short id_centro, short area_tecnica, List<short> id_roles = null)
        {
            try
            {
                // Context.EMPLEADO.Where(w => w.ID_CENTRO == id_centro && w.DEPARTAMENTO.ID_TECNICA == area_tecnica).SelectMany(s => s.USUARIO);
                var predicate = PredicateBuilder.True<EMPLEADO>();
                var predicateOr = PredicateBuilder.False<EMPLEADO>();
                if (id_roles != null && id_roles.Count > 0)
                {
                    foreach (var item in id_roles)
                        predicateOr = predicateOr.Or(w => w.USUARIO.Any(a1 => a1.USUARIO_ROL.Any(a2 => a2.ID_ROL == item && a2.ID_CENTRO==id_centro)));
                    predicate = predicate.And(predicateOr.Expand());
                }
                predicate = predicate.And(w => w.ID_CENTRO == id_centro && w.DEPARTAMENTO.DEPARTAMENTO_AREA_TECNICA.Any(a => a.ID_TECNICA == area_tecnica));
                return Context.EMPLEADO.Where(predicate.Expand()).SelectMany(s => s.USUARIO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<USUARIO> ObtenerTodos(string Usuario = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<USUARIO>();
                if (!string.IsNullOrEmpty(Usuario))
                    predicate = predicate.And(w => w.ID_USUARIO.Contains(Usuario) || w.EMPLEADO.PERSONA.PATERNO.Contains(Usuario) || w.EMPLEADO.PERSONA.MATERNO.Contains(Usuario) || w.EMPLEADO.PERSONA.NOMBRE.Contains(Usuario));
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
        /// <returns>objeto de tipo "ETNIA"</returns>
        public IQueryable<USUARIO> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_PERSONA == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public USUARIO Obtener(string Usuario)
        {
            try
            {
                return GetData().Where(w => w.ID_USUARIO.Trim() == Usuario.Trim()).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public USUARIO ObtenerUsuario(string Usuario)
        {
            try
            {
                return GetData().Where(w => w.ID_USUARIO.Trim() == Usuario.Trim()).SingleOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(USUARIO Entity, string contrasenia)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions(){IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted}))
                {
                    Context.USUARIO.Add(Entity);
                    //Crea usuario de BD
                    //Context.CREARUSUARIO(Entity.ID_USUARIO, contrasenia);
                    //creamos usuario
                    Context.Database.ExecuteSqlCommand(string.Format("CREATE USER {0} IDENTIFIED BY {1}",Entity.ID_USUARIO, contrasenia));
                    //creamos permiso
                    Context.Database.ExecuteSqlCommand(string.Format("GRANT create session TO {0}", Entity.ID_USUARIO));
                    //Ejecutamos permisos
                    #region Permisos
                    var permisos = "BEGIN " +
                    "FOR R IN (SELECT owner, table_name FROM all_tables WHERE owner='SSP') LOOP " +
                    "EXECUTE IMMEDIATE 'GRANT SELECT, INSERT, DELETE, UPDATE on '||R.owner||'.'||R.table_name||' to " + Entity.ID_USUARIO + "'; " +
                    "END LOOP; "+
                    "END; "; 
                    Context.Database.ExecuteSqlCommand(permisos);
                    #endregion
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                #region comentado
                //if (Insert(Entity))
                //{
                //    var res = CreateUserDB(Entity.ID_USUARIO, contrasenia);
                //    return true;
                //}
                #endregion
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(USUARIO Entity,string contrasenia)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = EntityState.Modified;
                    //Context.CREARUSUARIO(Entity.ID_USUARIO.Trim(), contrasenia);
                    //creamos usuario
                    //Context.Database.ExecuteSqlCommand(string.Format("CREATE USER {0} IDENTIFIED BY {1}", Entity.ID_USUARIO, contrasenia));
                    //creamos permiso
                    //Context.Database.ExecuteSqlCommand(string.Format("GRANT create session TO {0}", Entity.ID_USUARIO));
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Update(Entity))
                //{
                //    //var res = UpdatePassUserDB(Entity.ID_USUARIO, contrasenia);
                //    return true;
                //}
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
        public bool Eliminar(USUARIO Entity)
        {
            try
            {
                if (Delete(Entity))
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


        public IEnumerable<cReportePlantillaPersonalTecnico> ObtenerReportePlantillaPersonalTecnico(short Centro)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT ");
                query.Append("D.ID_DEPARTAMENTO,D.DESCR DEPARTAMENTO,SR.ID_ROL,SR.DESCR ROL, ");
                query.Append("COUNT(U.ID_USUARIO) AS TOTAL ");
                query.Append("FROM  ");
                query.Append("SSP.DEPARTAMENTO D ");
                query.Append("INNER JOIN SSP.SISTEMA_ROL SR ON D.ID_ROL = SR.ID_ROL ");
                query.Append("INNER JOIN SSP.USUARIO_ROL UR ON SR.ID_ROL = UR.ID_ROL ");
                query.Append("INNER JOIN SSP.USUARIO U ON UR.ID_USUARIO = U.ID_USUARIO ");
                query.AppendFormat("WHERE UR.ID_CENTRO = {0} AND U.ESTATUS = 'S' ",Centro);
                query.Append("GROUP BY D.ID_DEPARTAMENTO,D.DESCR,SR.ID_ROL,SR.DESCR ");
                query.Append("ORDER BY D.ID_DEPARTAMENTO,SR.ID_ROL ");
                return Context.Database.SqlQuery<cReportePlantillaPersonalTecnico>(query.ToString());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }

    public class cReportePlantillaPersonalTecnico
    { 
        public short ID_DEPARTAMENTO {get;set;}
        public string DEPARTAMENTO { get; set; }
        public short ID_ROL { get; set; }
        public string ROL { get; set; }
        public short TOTAL { get; set; }
    }
}