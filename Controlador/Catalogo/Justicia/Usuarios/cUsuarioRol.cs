using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cUsuarioRol : EntityManagerServer<USUARIO_ROL>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cUsuarioRol()
        { }
        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<USUARIO_ROL> ObtenerTodos(string Usuario = "",short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<USUARIO_ROL>();
                if (!string.IsNullOrEmpty(Usuario))
                    predicate = predicate.And(w => w.ID_USUARIO.Trim() == Usuario);
                if(Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                predicate = predicate.And(w => w.ID_ROL != 0);//sin rol
                
             
                return GetData(predicate.Expand());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }            
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos usuarios con rol de medico
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<USUARIO_ROL> ObtenerTodosMedicos(string Usuario = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<USUARIO_ROL>();
                if (!string.IsNullOrEmpty(Usuario))
                    predicate = predicate.And(w => w.ID_USUARIO.Trim() == Usuario);

                predicate = predicate.And(w => w.ID_ROL == 29 || w.ID_ROL == 30);//sin rol

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
        public IQueryable<USUARIO_ROL> Obtener(string Usuario,short Rol)
        {
            try
            {
                return GetData().Where(w => w.ID_USUARIO.Trim() == Usuario && w.ID_ROL == Rol);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(USUARIO_ROL Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (!Context.USUARIO_ROL.Any(w => w.ID_USUARIO == Entity.ID_USUARIO && w.ID_ROL == Entity.ID_ROL && w.ID_CENTRO == Entity.ID_CENTRO))
                    {
                        Context.USUARIO_ROL.Add(Entity);
                        var roles = Context.SISTEMA_ROL.Where(w => w.ID_ROL == Entity.ID_ROL).FirstOrDefault();
                        if (roles != null)
                        {
                            if (roles.PROCESO_ROL != null)
                            {
                                foreach (var p in roles.PROCESO_ROL)
                                {
                                    Context.PROCESO_USUARIO.Add(new PROCESO_USUARIO() { ID_USUARIO = Entity.ID_USUARIO, ID_PROCESO = p.ID_PROCESO, ID_CENTRO = Entity.ID_CENTRO, ID_ROL = Entity.ID_ROL, INSERTAR = p.INSERTAR, EDITAR = p.EDITAR, CONSULTAR = p.CONSULTAR, IMPRIMIR = p.IMPRIMIR });
                                }
                            }
                        }
                        Context.SaveChanges();
                        transaccion.Complete();
                    }
                    return true;
                }
                #region Comentado
                //if (Insert(Entity))
                //{
                //    //asignamos los procesos al usuario

                //    var rol = new cSistemaRol().Obtener(Entity.ID_ROL);
                //    if (rol != null)
                //    {
                //        List<PROCESO_USUARIO> list = new List<PROCESO_USUARIO>();
                //        foreach (var p in rol.PROCESO_ROL)
                //        {
                //            list.Add(new PROCESO_USUARIO() { ID_USUARIO = Entity.ID_USUARIO, ID_PROCESO = p.ID_PROCESO, ID_ROL = Entity.ID_ROL, INSERTAR = p.INSERTAR, EDITAR = p.EDITAR, CONSULTAR = p.CONSULTAR, IMPRIMIR = p.IMPRIMIR });
                //        }
                //        if (new cProcesoUsuario().Insertar(list, Entity.ID_USUARIO))
                //            return true;
                //    }
                //    else
                //        return true;
                //}
                #endregion
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
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(USUARIO_ROL Entity)
        {
            try
            {
                if (Update(Entity))
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
        public bool Eliminar(USUARIO_ROL Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //eliminamos los procesos de usuarios
                    var procesos = Context.PROCESO_USUARIO.Where(w => w.ID_USUARIO == Entity.ID_USUARIO && w.ID_ROL == Entity.ID_ROL && w.ID_CENTRO == Entity.ID_CENTRO);
                    if (procesos != null)
                    {
                        foreach (var p in procesos)
                        {
                            Context.PROCESO_USUARIO.Remove(p);
                        }
                    }
                    //Eliminamos el rol
                    Context.USUARIO_ROL.Attach(Entity);
                    Context.USUARIO_ROL.Remove(Entity);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                #region Comentado
                //var proc = new cProcesoUsuario().ObtenerTodos(Entity.ID_USUARIO, Entity.ID_ROL);
                //if (proc != null)
                //{
                //    List<PROCESO_USUARIO> list = new List<PROCESO_USUARIO>();
                //    foreach (var pu in proc)
                //    {
                //        list.Add(new PROCESO_USUARIO() { ID_PROCESO = pu.ID_PROCESO, ID_USUARIO = pu.ID_USUARIO, ID_ROL = pu.ID_ROL });
                //    }
                //    new cProcesoUsuario().Delete(list);
                //}
                ////var rol = new cSistemaRol().Obtener(Entity.ID_ROL).FirstOrDefault();
                ////if (rol != null)
                ////{
                ////    if (rol.PROCESO_ROL != null)
                ////    {
                ////        List<PROCESO_USUARIO> procesos = new List<PROCESO_USUARIO>();
                ////        foreach (var p in rol.PROCESO_ROL)
                ////        {
                ////            if (proc.Any())
                ////            {
                ////                if(proc.Where(w => w.ID_PROCESO == p.ID_PROCESO).Any())
                ////                    procesos.Add(new PROCESO_USUARIO { ID_USUARIO = Entity.ID_USUARIO, ID_PROCESO = p.ID_PROCESO });
                ////            }
                ////        }
                ////        new cProcesoUsuario().Delete(procesos);
                ////    }
                ////}
                //if (Delete(Entity))
                //    return true;
                #endregion
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