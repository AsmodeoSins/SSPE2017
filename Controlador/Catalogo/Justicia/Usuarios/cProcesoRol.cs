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
    public class cProcesoRol : EntityManagerServer<PROCESO_ROL>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cProcesoRol()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "AREA_VISITA"</returns>
        public IQueryable<PROCESO_ROL> ObtenerTodos(short? Rol = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO_ROL>();
                if (Rol != null)
                    predicate = predicate.And(w => w.ID_ROL == Rol);
                return GetData(predicate.Expand());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }            
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public IQueryable<PROCESO_ROL> Obtener(short Proceso,short Rol)
        {
            try
            {
                return GetData().Where(w => w.ID_ROL == Rol && w.ID_PROCESO == Proceso);
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
        public IQueryable<PROCESO_ROL> Obtener(List<SISTEMA_ROL> list)
        {
            try
            {
                var predicate = PredicateBuilder.True<PROCESO_ROL>();
                bool band = true;
                if (list != null)
                {
                    foreach (var l in list)
                    {
                        if (band)
                        {
                            predicate = predicate.And(w => w.ID_ROL == l.ID_ROL);
                            band = false;
                        }
                        else
                            predicate = predicate.Or(w => w.ID_ROL == l.ID_ROL);
                    }
                }
                return GetData(predicate.Expand()).OrderBy(w => w.ID_ROL);;
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
        public bool Insertar(PROCESO_ROL Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    Context.PROCESO_ROL.Add(Entity);
                    //Obtenemos los usuarios con el Rol
                    var usuarios = Context.USUARIO_ROL.Where(w => w.ID_ROL == Entity.ID_ROL);
                    if (usuarios != null)
                    {
                        foreach (var usr in usuarios)
                        {
                            Context.PROCESO_USUARIO.Add(new PROCESO_USUARIO()
                            {
                             ID_PROCESO = Entity.ID_PROCESO,
                             ID_USUARIO = usr.ID_USUARIO,
                             ID_ROL = Entity.ID_ROL,
                             INSERTAR = Entity.INSERTAR,
                             EDITAR = Entity.EDITAR,
                             CONSULTAR = Entity.CONSULTAR,
                             IMPRIMIR = Entity.IMPRIMIR,
                             DIGITALIZAR = Entity.DIGITALIZAR,
                             ID_CENTRO = usr.ID_CENTRO
                            });
                        }
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
                //if (Insert(Entity))
                //    return true;
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
        public bool Actualizar(PROCESO_ROL Entity)
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
        public bool Eliminar(PROCESO_ROL Entity)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(Entity).State = System.Data.EntityState.Deleted;
                    //Context.PROCESO_ROL.Remove(Entity);
                    //Removemos el permiso de los usuarios
                    var usuarios = Context.PROCESO_USUARIO.Where(w => w.ID_ROL == Entity.ID_ROL);
                    if (usuarios != null)
                    { 
                       foreach(var usr in usuarios)
                       {
                           //var pu = new PROCESO_USUARIO();
                           //pu.ID_CENTRO = usr.ID_CENTRO;
                           //pu.ID_PROCESO = usr.ID_PROCESO;//Entity.ID_PROCESO;
                           //pu.ID_USUARIO = usr.ID_USUARIO;
                           //pu.ID_ROL = usr.ID_ROL;//Entity.ID_ROL;
                           Context.Entry(usr).State = System.Data.EntityState.Deleted;
                           //Context.PROCESO_USUARIO.Remove(pu);
                       }
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                }
                //if (Delete(Entity))
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